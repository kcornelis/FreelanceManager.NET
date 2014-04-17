require 'nokogiri'

def config(*args)
    args ||= []
        
    data = Struct.new(:files, :appSettings, :connectionStrings, :xpath).new
    yield(data)
    
    body = proc {
        ConfigTask.new.execute(data.files, data.appSettings, data.connectionStrings, data.xpath)
    }
    
    Rake::Task.define_task(*args, &body)
end

class ConfigTask
    def execute(files, appSettings, connectionStrings, xpath)
        files.each do |file|
            doc = Nokogiri::XML(File.open(file))

            unless appSettings.nil?
                appSettings.each do |key,value|
                    doc.xpath('/configuration/appSettings/add[@key="' + key + '"]').each do |el|
                        el['value'] = value
                    end
                end
            end
			
			unless connectionStrings.nil?
                connectionStrings.each do |key,value|
                    doc.xpath('/configuration/connectionStrings/add[@name="' + key + '"]').each do |el|
                        el['connectionString'] = value
                    end
                end
            end

            unless xpath.nil?
                xpath.each do |key, params|
                    doc.xpath(key, params.namespaces).each do |el|
                        el.content = params.value
                    end
                end
            end

            File.open(file, 'w') { |f| f.write(doc) }
        end
    end
end