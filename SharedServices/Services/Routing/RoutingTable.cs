using SharedInterfaces.Interfaces.Routing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharedServices.Services.Routing
{
    public class RoutingTable<T> : IRoutingTable<T>
    {
        public IMessageBusBank<T> MessageBusBank { get; set; }
        public string ExceptionMessage_RouteCannotBeNullOrEmpty
        {
            get
            {
                return "RoutingTable<T> - Route cannot be null or empty.";
            }
        }
        public string ExceptionMessage_RouteFormatIsIncorrect
        {
            get
            {
                return "RoutingTable<T> - Route format is incorrect, it should be: <RouterBusKeyCode>.<ServiceMethodCode> example 1: yourstring.yourstring.yourstring  example 2: 102.362.BE60675D-DB24-45A2-925C-00C5DC753C92";
            }
        }
        public string ExceptionMessage_RouteActionCannotBeNull
        {
            get
            {
                return "RoutingTable<T> - RouteAction cannot be null.";
            }
        }
        public string ExceptionMessage_MessageBusBankCannotBeNull
        {
            get
            {
                return "RoutingTable<T> - MessageBusBank cannot be null.";
            }
        }
        private Dictionary<string, Action<T>> _routeTable { get; set; } 
        private string _routingTableGUID { get; set; }
        public string RoutingTableGUID
        {
            get
            {
                if (String.IsNullOrEmpty(_routingTableGUID))
                    _routingTableGUID = Guid.NewGuid().ToString();
                return _routingTableGUID;
            }
        }
        private bool _isDisposed { get; set; }

        public RoutingTable()
        {
            _routeTable = new Dictionary<string, Action<T>>();
            _isDisposed = false;
        }

        public void Dispose()
        {
            if (_isDisposed == false)
            {
                _routeTable.Clear();
                //TODO: Notify all subscribers of shutdown before clearing. 
                _isDisposed = true;
            }
        }

        public bool RegisterRoute(string route, Action<T> routeAction)
        { 
            try
            {
                if (String.IsNullOrEmpty(route))
                    throw new InvalidOperationException(ExceptionMessage_RouteCannotBeNullOrEmpty);
                else if (routeAction == null)
                    throw new InvalidOperationException(ExceptionMessage_RouteActionCannotBeNull);
                else if (route.Split('.').Count() != 2)
                    throw new InvalidOperationException(ExceptionMessage_RouteFormatIsIncorrect);
                else
                {
                    _routeTable.Add(route, routeAction);
                    return true;
                } 
            }
            catch(InvalidOperationException ex)
            {
                throw new InvalidOperationException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        } 

        public Action<T> ResolveRoute(string route)
        { 
            try
            {
                Action<T> resolvedRoute = null;
                if (String.IsNullOrEmpty(route))
                    throw new InvalidOperationException(ExceptionMessage_RouteCannotBeNullOrEmpty);
                else if (route.Split('.').Count() != 2)
                    throw new InvalidOperationException(ExceptionMessage_RouteFormatIsIncorrect);
                else if (MessageBusBank == null)
                    throw new InvalidOperationException(ExceptionMessage_MessageBusBankCannotBeNull);
                else if (route.Split('.').ElementAt(0) != RoutingTableGUID)
                {
                    Action<T> forwardedRoute =
                        (message) =>
                        {
                            string busKeyCode = route.Split('.').ElementAt(0); 
                            MessageBusBank.ResolveMessageBus(busKeyCode)
                            .SendMessage(message);
                        };
                    return forwardedRoute;
                }
                else
                {
                    _routeTable.TryGetValue(route, out resolvedRoute);
                    return resolvedRoute;
                } 
            }
            catch(InvalidOperationException ex)
            {
                throw new InvalidOperationException(ex.Message, ex);
            }
            catch (Exception ex)
            { 
                throw new ApplicationException(ex.Message, ex);
            }
        }

        public bool ReleaseRoute(string route)
        { 
            try
            {
                if (String.IsNullOrEmpty(route))
                    throw new InvalidOperationException(ExceptionMessage_RouteCannotBeNullOrEmpty);
                else if (route.Split('.').Count() != 2)
                    throw new InvalidOperationException(ExceptionMessage_RouteFormatIsIncorrect);
                else
                {
                    return _routeTable.Remove(route);
                }               
            }
            catch(InvalidOperationException ex)
            {
                throw new InvalidOperationException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        }
    }
}
