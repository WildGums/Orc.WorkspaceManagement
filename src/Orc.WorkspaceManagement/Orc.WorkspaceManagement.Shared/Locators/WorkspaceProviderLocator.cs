// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceProviderLocator.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel.Threading;

    public class WorkspaceProviderLocator : IWorkspaceProviderLocator
    {
/*        private readonly IServiceLocator _serviceLocator;
        private readonly ITypeFactory _typeFactory;

        public WorkspaceProviderLocator(ITypeFactory typeFactory, IServiceLocator serviceLocator)
        {
            Argument.IsNotNull(() => typeFactory);

            _typeFactory = typeFactory;
            _serviceLocator = serviceLocator;
        }*/

        public virtual Task<IEnumerable<IWorkspaceProvider>> ResolveAllWorkspaceProvidersAsync(object tag = null)
        {
            /*var providers = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypesEx())
                .Where(type => type.BaseType.IsAssignableFromEx(typeof (IWorkspaceProvider)))
                .Select(type => ResolveOrCreateInstance(type, tag)).OfType<IWorkspaceProvider>();

            return TaskHelper<IEnumerable<IWorkspaceProvider>>.FromResult(providers.ToArray());*/

            return TaskHelper<IEnumerable<IWorkspaceProvider>>.FromResult(Enumerable.Empty<IWorkspaceProvider>());
        }

        /*  protected object ResolveOrCreateInstance(Type type, object tag = null)
        {
            Argument.IsNotNull(() => type);

            var instance = _serviceLocator.ResolveType(type, tag);
            if (instance == null)
            {
                instance = _typeFactory.CreateInstance(type);
                _serviceLocator.RegisterInstance(instance, tag);
            }

            return instance;
        }*/
    }
}