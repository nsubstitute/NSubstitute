using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NSubstitute.Routing;
using NSubstitute.Routing.Definitions;
using NSubstitute.Routing.Handlers;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Routing.Definitions
{
    public class RouteDefininitionSpecs
    {
        public class When_getting_handlers_for_defined_routes : StaticConcern
        {
            IDictionary<Type, IEnumerable<Type>> _definitionToHandlersMap = new Dictionary<Type, IEnumerable<Type>>{
                { typeof(CheckCallNotReceivedRoute), 
                    new[] { typeof(CheckDidNotReceiveCallHandler), typeof(ReturnDefaultForReturnTypeHandler)}
                    },
                { typeof(CheckCallReceivedRoute),
                    new[] { typeof(CheckReceivedCallHandler), typeof(ReturnDefaultForReturnTypeHandler) }
                    },
                { typeof(DoWhenCalledRoute),
                    new[] {typeof(SetActionForCallHandler), typeof(ReturnDefaultForReturnTypeHandler)} 
                    },
                { typeof(RaiseEventRoute),
                    new[] {typeof (RaiseEventHandler), typeof (ReturnDefaultForReturnTypeHandler) }
                    },
                { typeof(RecordReplayRoute),
                    new[] { typeof (EventSubscriptionHandler), typeof (PropertySetterHandler),
                            typeof (DoActionsCallHandler), typeof (RecordCallHandler), 
                            typeof (ReturnConfiguredResultHandler), typeof(ReturnDefaultForReturnTypeHandler) }
                    },
            };

            [Test]
            public void Should_have_required_handler_types()
            {
                foreach (var definitionType in GetAllRouteDefinitionTypes())
                {
                    var actualHandlers = CreateDefinition(definitionType).HandlerTypes;
                    var requiredHandlers = GetHandlersForDefinitionType(definitionType);
                    Assert.That(requiredHandlers, Is.EqualTo(actualHandlers), definitionType.Name + " did not have required handlers.");
                }
            }

            private IEnumerable<Type> GetAllRouteDefinitionTypes()
            {
                var assemblyWithRouteDefinitions = Assembly.GetAssembly(typeof(IRouteDefinition));
                return
                    assemblyWithRouteDefinitions
                    .GetTypes()
                    .Where(x => typeof(IRouteDefinition).IsAssignableFrom(x) && x != typeof(IRouteDefinition));
            }

            private IEnumerable<Type> GetHandlersForDefinitionType(Type definitionType)
            {
                IEnumerable<Type> handlerTypes;
                if (_definitionToHandlersMap.TryGetValue(definitionType, out handlerTypes))
                {
                    return handlerTypes;
                }
                throw new AssertionException("No expected handler types have been specified for " + definitionType.Name + ". Add expected types to dictionary.");
            }

            IRouteDefinition CreateDefinition(Type definitionType)
            {
                return (IRouteDefinition)Activator.CreateInstance(definitionType);
            }
        }
    }
}