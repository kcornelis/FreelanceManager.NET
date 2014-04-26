require 'albacore'
require './Build/ConfigHelper.rb'
require "azure"
require 'win32/service'
include Win32

task :default => [ :build ]
task :build => [ :config_setup, :nlog_setup, :compile, :publish ]
task :deploy => [ :build, :deployLocal ]


# Good tutorials
# http://daniel.wertheim.se/2011/07/14/continuous-integration-using-teamcity-rake-albacore-github-and-nunit-for-net-%E2%80%93-part-3/

#
# Environment Parameters
#
@env_version = ENV['env_buildversion'] || '1.0.0.0'
@env_loglevel = ENV['env_loglevel'] || 'Debug'
@env_localReadModelServiceDir = ENV['env_localReadModelServiceDir'] || 'C:\Test\ReadModelService'
@env_localReadModelServiceName = ENV['env_localReadModelServiceName'] || 'FreelanceManagerReadModel'
@env_localWebsiteDir = ENV['env_localWebsiteDir'] || 'C:\Test\Websites'

@env_azureServiceBusNamespace = ENV['env_azureServiceBusNamespace'] || ''
@env_azureServiceBusIssuerName = ENV['env_azureServiceBusIssuerName'] || ''
@env_azureServiceBusIssuerKey = ENV['env_azureServiceBusIssuerKey'] || ''

@env_connectionReadModel = ENV['env_connectionReadModel'] || 'mongodb://localhost/FM-ReadModel'
@env_connectionEventStore = ENV['env_connectionEventStore'] || 'mongodb://localhost/FM-EventStore'
@env_connectionLoggingHost = ENV['env_connectionLogging'] || 'localhost'

#
# Parameters
# 
@par_basedir = Dir.getwd + '/'
@par_publishdir = "#{@par_basedir}Published/"

#
# Tasks
#
desc "Init"
task :clean do
	puts 'Base dir: ' + @par_basedir
	FileUtils.rm_rf("#{@par_publishdir}/.", secure: true)
end

desc "Config"
config :config_setup => :clean do |data|
	puts "Configuration"

	data.files = FileList['./FreelanceManager.Web/Web.config', './FreelanceManager.ReadModel.Console/app.config', './FreelanceManager.ReadModel.WindowsService/app.config']
	data.appSettings = {
		'azure:serviceBusNamespace' => @env_azureServiceBusNamespace,
        'azure:serviceBusIssuerName' => @env_azureServiceBusIssuerName,
        'azure:serviceBusIssuerKey' => @env_azureServiceBusIssuerKey
    }
	data.connectionStrings = {
		'MongoConnectionReadModel' => @env_connectionReadModel,
		'MongoConnectionEventStore' => @env_connectionEventStore
	}
end

desc "NLog config"
config :nlog_setup => :clean do |data|
	puts "Nlog configuration"

	data.files = FileList['./FreelanceManager.Web/Nlog.config', './FreelanceManager.ReadModel.Console/Nlog.config', './FreelanceManager.ReadModel.WindowsService/Nlog.config']
    data.xpath = {
    	"//./x:target[@name='Mongo']/@host" => OpenStruct.new(:value => @env_connectionLoggingHost, :namespaces => { 'x' => "http://www.nlog-project.org/schemas/NLog.xsd" })
    	#,"//./x:logger/@minlevel" => OpenStruct.new(:value => @env_loglevel, :namespaces => { 'x' => "http://www.nlog-project.org/schemas/NLog.xsd" }) 
    }
end

desc "Versioning"
task :versioning do
	puts "Edit assembly info"

	assembly_info_files = FileList["./*/Properties/AssemblyInfo.cs"]
	assembly_info_files.each do |file|
		asm = AssemblyInfo.new()
		asm.use(file)
		asm.version = @env_version
		asm.file_version = @env_version

		asm.company_name = "Kevin Cornelis"
		asm.product_name = "Freelance manager"
		asm.copyright = "Kevin Cornelis (c) 2014"

		asm.execute
	end
end

desc "Compile"
msbuild :compile => :versioning do |msb|
	puts "Building the solution"

	msb.properties = { :configuration => :Release, 
					   :VisualStudioVersion => '12.0' }
	msb.targets = [ :Clean, :Build ]
	msb.solution = "FreelanceManager.sln"
end

desc "Publish"
msbuild :publish => :versioning  do |msb|
  puts "Publishing the website"
  msb.properties = {:configuration => :Release,
  					:PublishProfile => :BuildServer, 
					:VisualStudioVersion => '12.0' }
  msb.targets =[ :WebPublish ]
  msb.solution = "FreelanceManager.Web/FreelanceManager.Web.csproj"
end

desc "Deploy Local"
task :deployLocal do
	puts "Local deploy"

	if Service.exists?(@env_localReadModelServiceName) && Service.status(@env_localReadModelServiceName).current_state == 'running'
		Service.stop(@env_localReadModelServiceName)
		while Service.status(@env_localReadModelServiceName).current_state != 'stopped'
            sleep 5
         end
	end

	FileUtils.cp_r( Dir[@par_publishdir + 'Web/*'], @env_localWebsiteDir)
	FileUtils.cp_r( Dir[@par_basedir + 'FreelanceManager.ReadModel.WindowsService/bin/Release/*'], @env_localReadModelServiceDir)
	
	if Service.exists?(@env_localReadModelServiceName) == false
		puts "#{@env_localReadModelServiceDir}\\FreelanceManager.ReadModel.WindowsService.exe"
		Service.new(
	        :service_name     => @env_localReadModelServiceName,
	        :display_name     => @env_localReadModelServiceName,
	        :description      => "FreelanceManager Read Model Service",
	        :binary_path_name => "#{@env_localReadModelServiceDir}\\FreelanceManager.ReadModel.WindowsService.exe"
      	)
	end

	Service.start(@env_localReadModelServiceName)
end