using Microsoft.Extensions.Localization;
using OpenMod.API.Plugins;
using SilK.Unturned.Extras.Accessors;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace SilK.Unturned.Extras.Localization
{
    internal class StringLocalizerAccessor<TPlugin> :
        PluginServiceAccessor<TPlugin, IStringLocalizer>,
        IStringLocalizerAccessor<TPlugin> where TPlugin : IOpenModPlugin
    {
        public StringLocalizerAccessor(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        /// <inheritdoc cref="IStringLocalizer"/>
        public LocalizedString this[string name] => GetInstance()[name];

        /// <inheritdoc cref="IStringLocalizer"/>
        public LocalizedString this[string name, params object[] arguments] => GetInstance()[name, arguments];

        /// <inheritdoc cref="IStringLocalizer"/>
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) =>
            GetInstance().GetAllStrings(includeParentCultures);

        /// <inheritdoc cref="IStringLocalizer"/>
        [Obsolete("This method is obsolete. Use `CurrentCulture` and `CurrentUICulture` instead.")]
        public IStringLocalizer WithCulture(CultureInfo culture) => GetInstance(); //.WithCulture(culture); No longer available
    }
}
