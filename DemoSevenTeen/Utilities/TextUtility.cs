using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoSevenTeen.Utilities
{
    internal static class TextUtility
    {
        public static string GetCsvText(string input, bool hasHeader = false)
        {
            var items = GetCopiedTextAsArray(input);

            if(hasHeader)
            {
                items = RemoveHeader(items);
            }

            var inputTextValueType = GetCollectionValueType(items);

            return JoinItems(items, inputTextValueType);
        }

        public static TextValueType GetSingleValueType(string item)
        {

            if (item.All<char>(x => char.IsNumber(x)))
                return TextValueType.Numeric;

            Guid guidResult;
            if (Guid.TryParse(item, out guidResult))
                return TextValueType.Guid;

            DateTime dateTimeResult;
            if (DateTime.TryParse(item, out dateTimeResult))
                return TextValueType.DateTime;

            return TextValueType.Text;
        }

        /// <summary>
        /// Returns the copied columns/rows as a string array.
        /// </summary>
        /// <param name="copiedText">Copied Text to Seperate</param>
        /// <param name="removeEmptyStringValues">Set to <c>true</c> to remove cells with empty strings. Default: <c>false</c></param>
        /// <returns>Array of Cell Content Strings.</returns>
        public static string[] GetCopiedTextAsArray(string copiedText, bool removeEmptyStrings = false)
        {
            var splitOptions = removeEmptyStrings ? StringSplitOptions.None : StringSplitOptions.RemoveEmptyEntries;
            var cellValues = copiedText.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            return RemoveCellsWithNull(cellValues);
        }

        /// <summary>
        /// Remove 'NULL' cells from Copied Text.
        /// </summary>
        /// <param name="items">Array of string to parse and remove "NULL" values.</param>
        /// <returns>Array of strings without "NULL" values.</returns>
        private static string[] RemoveCellsWithNull(string[] items)
        {
            var filteredItems = new List<string>();

            foreach (var item in items)
            {
                // Set NULL as Upper case based on SSMS Default output.
                if (!item.Equals("NULL"))
                {
                    filteredItems.Add(item);
                }
            }
            return filteredItems.ToArray();
        }

        public static string[] RemoveHeader(string[] items)
        {
            var itemsMinusHeader = new string[items.Length - 1];
            Array.Copy(items, itemsMinusHeader, items.Length - 1);
            return itemsMinusHeader;
        }

        public static TextValueType GetCollectionValueType(string[] items)
        {
            if (items.Length == 1)
            {
                return GetSingleValueType(items[0]);
            }

            var initialType = GetSingleValueType(items[items.Length - 1]);
            if (initialType == TextValueType.Text)
            {
                // If last item is Text, assume all are text.
                return TextValueType.Text;
            }

            for (int i = 0; i < items.Length - 2; i++)
            {
                // Todo: Extend to case value to optimize.

                if (GetSingleValueType(items[i]) != initialType)
                {
                    return TextValueType.Text;
                }
            }
            return initialType;
        }

        public static string JoinItems(string[] items, TextValueType textType)
        {
            var sb = new StringBuilder();

            FormatCollection(ref sb, items, textType);

            return sb.ToString();
        }

        public static void FormatItem(ref StringBuilder sb, string item, TextValueType itemType, LineFormatType lineType)
        {
            switch(itemType)
            {
                case TextValueType.Guid:
                case TextValueType.DateTime:
                    item = $"'{item}'";
                    break;
                case TextValueType.Text:
                    item = $"N'{item}'";
                    break;
                case TextValueType.Numeric:
                    break;
                default:
                    throw new Exception("Error Getting TextValueType");
            }

            switch(lineType)
            {
                case LineFormatType.Even:
                    sb.Append($"{item}, ");
                    break;
                case LineFormatType.Odd:
                    sb.AppendLine($"{item}, ");
                    break;
                case LineFormatType.Final:
                    sb.Append($"{item}");
                    break;
                default:
                    throw new Exception("Error Getting LineFormatType");
            }
        }

        public static void FormatCollection(ref StringBuilder sb, string[] items, TextValueType itemType)
        {
            for (int i = 0; i < items.Length - 1; i++)
            {
                if (i % 2 == 0)
                {
                    FormatItem(ref sb, items[i], itemType, LineFormatType.Even);
                }
                else
                {
                    FormatItem(ref sb, items[i], itemType, LineFormatType.Odd);
                }
            }
            FormatItem(ref sb, items[items.Length - 1], itemType, LineFormatType.Final);
        }
    }

    internal enum TextValueType
    {
        Unknown = 0,
        Text = 1,
        Numeric = 2,
        Guid = 3,
        DateTime = 4,
        TextWithHeader = 5,
        NumericWithHeader = 6,
        GuidWithHeader = 7,
        DateTimeWithHeader = 8,
        NullValue = 9
    }

    internal enum LineFormatType
    {
        Even,
        Odd,
        Final
    }
}
