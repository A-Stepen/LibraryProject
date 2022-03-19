using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace LibraryProject
{
    public class InjectableControllerActivator : IControllerActivator
    {
        Dictionary<Type, Func<IController>> factories;

        public InjectableControllerActivator(Dictionary<Type, Func<IController>> factories)
        {
            this.factories = factories;
        }

        public IController Create(RequestContext requestContext, Type controllerType)
        {
            if (factories.TryGetValue(controllerType, out Func<IController> method))
            {
                return method();
            }

            return (IController)Activator.CreateInstance(controllerType);
        }
    }
}