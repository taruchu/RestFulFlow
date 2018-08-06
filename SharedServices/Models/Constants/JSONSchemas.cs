

namespace SharedServices.Models.Constants
{
    public class JSONSchemas
    {
        public static string DestinationRoute = "DestinationRoute";
        public static string SenderRoute = "SenderRoute"; 
        public static string PersistenceServiceCommand = "PersistenceServiceCommand";
        public static string StorageMechanism = "StorageMechanism";
        public static string StorageID = "StorageID";
        public static string StoragePayload = "StoragePayload";
        public static string ChatMessageChannelName = "ChatMessageChannelName";
        public static string ChatMessageServiceCommand = "ChatMessageServiceCommand";
        public static string ChatMessageBody = "ChatMessageBody";
        public static string ChatMessageGUID = "ChatMessageGUID";
        public static string ChatMessagePostedDateTimeStamp = "ChatMessagePostedDateTimeStamp";
        public static string ChatMessageSender = "ChatMessageSender";
        public static string ChatMessage = "ChatMessage";
        public static string ChatMessageTags = "ChatMessageTags";
        public static string RoutingServiceCommand = "RoutingServiceCommand";
        public static string RoutingServiceCommandRegister = "RoutingServiceCommandRegister";
        public static string RoutingServiceCommandRelease = "RoutingServiceCommandRelease";
        public static string Route = "Route";
        public static string ClientProxyOrigin = "ClientProxyOrigin";
        public static string RequestMethod = "RequestMethod";
        public static string WebServiceOriginUrl = "WebServiceOriginUrl";
        public static string ServiceNameRequested = "ServiceNameRequested";


        public static string RoutingServiceSchema = @"{
            '$schema': 'http://json-schema.org/draft-07/schema#',
            'description': 'Routing Service Envelope Schema',
            'type': 'object',
            'required': ['Header_KeyValues', 'Payload_KeyValues'],
            'minProperties': 2,
            'properties':
            {
                'Header_KeyValues': 
                {
                    'type': 'object',
                    'required': 
                    [
                        'SenderRoute' ,
                        'DestinationRoute' ,
                        'ClientProxyOrigin' ,
                        'RequestMethod' ,
                        'WebServiceOriginUrl',
                        'ServiceNameRequested'
                    ],
                    'maxProperties': 2,
                    'properties':
                    {
                        'SenderRoute': {'type': 'string'},
                        'DestinationRoute': {'type': 'string'},
                        'ClientProxyOrigin': {'type': 'string'},
                        'RequestMethod': {'type': 'string'},
                        'WebServiceOriginUrl': {'type': 'string'},
                        'ServiceNameRequested': {'type': 'string'}
                    }
                },
                'Payload_KeyValues': 
                {
                    'type': 'object',  
                    'required':
                    [
                        'RoutingServiceCommand', 
                        'Route'
                    ],
                    'maxProperties': 2,
                    'properties':
                    {
                        'RoutingServiceCommand': {'type': 'string'},
                        'Route': {'type': 'string'}
                    }
                }
            }
        }";

        //TODO: Need a way to create multiple schemas that will validate different types of service requests that are sent
        //to the same service. One request may not require the same key/value pairs as another. May end up using If/Else/Then 
        //within the schema for this, but that may get over-complex as the service methods grow.

        //NOTE: The command values used within the following service schemas are compositional JSON objects that encapsulate a command request. This way
        //the command is shaped like an object and can be free to contain whatever it needs. I'll store it as a string so that it's shape is flexible.

        //TODO: Need to document which Header properites can be left empty by the client verses which ones must contain valid values. Some properties will
        //receive values later in the pipeline. Perhaps create a client version of the schema that has less required properties ???

        public static string PersistenceServiceSchema = @"{
            '$schema': 'http://json-schema.org/draft-07/schema#',            
            'description': 'Persistence Service Envelope Schema',
            'type': 'object',
            'required': ['Header_KeyValues', 'Payload_KeyValues'],
            'minProperties': 2,
            'properties':
            {
                 'Header_KeyValues': 
                {
                    'type': 'object',
                    'required': 
                    [
                        'SenderRoute' ,
                        'DestinationRoute' ,
                        'ClientProxyOrigin' ,
                        'RequestMethod' ,
                        'WebServiceOriginUrl',
                        'ServiceNameRequested'
                    ],
                    'maxProperties': 2,
                    'properties':
                    {
                        'SenderRoute': {'type': 'string'},
                        'DestinationRoute': {'type': 'string'},
                        'ClientProxyOrigin': {'type': 'string'},
                        'RequestMethod': {'type': 'string'},
                        'WebServiceOriginUrl': {'type': 'string'},
                        'ServiceNameRequested': {'type': 'string'}
                    }
                },
                'Payload_KeyValues': 
                {
                    'type': 'object',
                    'required':
                    [
                        'PersistenceServiceCommand',
                        'StorageMechanism',
                        'StorageID',
                        'StoragePayload'
                    ],
                    'maxProperties': 3,
                    'properties':
                    {
                        'PersistenceServiceCommand': {'type': 'string'},
                        'StorageMechanism': {'type': 'string'},
                        'StorageID': {'type': 'string'},
                        'StoragePayload': {'type': 'string'}
                    }
                }
            }
        }";
        
        public static string ChatMessageServiceSchema = @"{
            '$schema': 'http://json-schema.org/draft-07/schema#',            
            'description': 'Chat Message Service Envelope Schema',
            'type': 'object',
            'required': ['Header_KeyValues', 'Payload_KeyValues'],
            'minProperties': 2,
            'properties':
            {
                'Header_KeyValues': 
                {
                    'type': 'object',
                    'required': 
                    [
                        'SenderRoute' ,
                        'DestinationRoute' ,
                        'ClientProxyOrigin' ,
                        'RequestMethod' ,
                        'WebServiceOriginUrl',
                        'ServiceNameRequested'
                    ],
                    'maxProperties': 2,
                    'properties':
                    {
                        'SenderRoute': {'type': 'string'},
                        'DestinationRoute': {'type': 'string'},
                        'ClientProxyOrigin': {'type': 'string'},
                        'RequestMethod': {'type': 'string'},
                        'WebServiceOriginUrl': {'type': 'string'},
                        'ServiceNameRequested': {'type': 'string'}
                    }
                },
                'Payload_KeyValues': 
                {
                    'type': 'object',
                    'required':
                    [
                        'ChatMessageChannelName', 
                        'ChatMessageServiceCommand',
                        'ChatMessageBody',
                        'ChatMessageGUID',
                        'ChatMessagePostedDateTimeStamp',
                        'ChatMessageSender',
                        'ChatMessage',
                        'ChatMessageTags'
                    ],
                    'maxProperties': 8,
                    'properties':
                    {
                        'ChatMessageChannelName': {'type': 'string'}, 
                        'ChatMessageServiceCommand': {'type': 'string'},
                        'ChatMessageSender': {'type': 'string'},
                        'SendNotification': {'type': 'string'},
                        'ChatMessageTags': {'type': 'string'},
                        'ChatMessageGUID': {'type': 'string'},
                        'ChatMessageBody': {'type': 'string'},
                        'ChatMessagePostedDateTimeStamp': {'type': 'string'}                         
                    }
                }
            }
        }";
    }
}
