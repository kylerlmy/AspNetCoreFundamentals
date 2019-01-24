using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OptionsExamples
{
    private readonly MyOptions _options;
    public class IndexModel
    {
        public IndexModel(IOptionsMonitor<MyOptions> optionsAccessor,
    IOptionsMonitor<MyOptionsWithDelegateConfig> optionsAccessorWithDelegateConfig,
    IOptionsMonitor<MySubOptions> subOptionsAccessor,
    IOptionsSnapshot<MyOptions> snapshotOptionsAccessor,
    IOptionsSnapshot<MyOptions> namedOptionsAccessor)
        {
            _options = optionsAccessor.CurrentValue;
            _optionsWithDelegateConfig = optionsAccessorWithDelegateConfig.CurrentValue;
            _subOptions = subOptionsAccessor.CurrentValue;
            _snapshotOptions = snapshotOptionsAccessor.Value;
            _named_options_1 = namedOptionsAccessor.Get("named_options_1");
            _named_options_2 = namedOptionsAccessor.Get("named_options_2");
        }

    }
}
