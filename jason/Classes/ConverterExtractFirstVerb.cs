using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Data;

namespace jason.Classes
{
    public class ConverterExtractFirstVerb : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            //if (value is List<lybra.Word> verbs && verbs.Any())
            //    return verbs[0].Term;
            //else
            //    return string.Empty;
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
