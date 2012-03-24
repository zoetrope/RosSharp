<?xml version="1.0" encoding="utf-8"?>
<configurationSectionModel xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="7e957972-e0df-4051-b03c-e53f3fb839f3" namespace="RosSharp" xmlSchemaNamespace="urn:RosSharp" assemblyName="RosSharp.NET40" xmlns="http://schemas.microsoft.com/dsltools/ConfigurationSectionDesigner">
  <typeDefinitions>
    <externalType name="String" namespace="System" />
    <externalType name="Boolean" namespace="System" />
    <externalType name="Int32" namespace="System" />
    <externalType name="Int64" namespace="System" />
    <externalType name="Single" namespace="System" />
    <externalType name="Double" namespace="System" />
    <externalType name="DateTime" namespace="System" />
    <externalType name="TimeSpan" namespace="System" />
  </typeDefinitions>
  <configurationElements>
    <configurationSection name="ConfigurationSection" codeGenOptions="Singleton, XmlnsProperty" xmlSectionName="rossharp">
      <elementProperties>
        <elementProperty name="Node" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="node" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/7e957972-e0df-4051-b03c-e53f3fb839f3/NodeElement" />
          </type>
        </elementProperty>
        <elementProperty name="Ros" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="ros" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/7e957972-e0df-4051-b03c-e53f3fb839f3/RosElement" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationSection>
    <configurationElement name="RosHostName">
      <attributeProperties>
        <attributeProperty name="Value" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="value" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/7e957972-e0df-4051-b03c-e53f3fb839f3/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElement name="NodeElement">
      <elementProperties>
        <elementProperty name="MasterUri" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="ROS_MASTER_URI" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/7e957972-e0df-4051-b03c-e53f3fb839f3/RosMasterUri" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationElement>
    <configurationElement name="RosElement">
      <elementProperties>
        <elementProperty name="HostName" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="ROS_HOSTNAME" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/7e957972-e0df-4051-b03c-e53f3fb839f3/RosHostName" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationElement>
    <configurationElement name="RosMasterUri">
      <attributeProperties>
        <attributeProperty name="Value" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="value" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/7e957972-e0df-4051-b03c-e53f3fb839f3/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
  </configurationElements>
  <propertyValidators>
    <validators />
  </propertyValidators>
</configurationSectionModel>