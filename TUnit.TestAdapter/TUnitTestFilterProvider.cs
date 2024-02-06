﻿using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using TUnit.TestAdapter.Constants;

namespace TUnit.TestAdapter;

internal class TUnitTestFilterProvider(IRunContext runContext, IMessageLogger messageLogger)
{
    private static readonly Dictionary<string, TestProperty> _supportedProperties =
        new Dictionary<string, TestProperty>(StringComparer.OrdinalIgnoreCase);

    static TUnitTestFilterProvider()
    {
        _supportedProperties[nameof(TUnitTestProperties.TestName)] =
            TUnitTestProperties.TestName;
        
        _supportedProperties[nameof(TUnitTestProperties.TestClass)] =
            TUnitTestProperties.TestClass;
        
        _supportedProperties[nameof(TUnitTestProperties.Category)] =
            TUnitTestProperties.Category;
        
        _supportedProperties[nameof(TUnitTestProperties.NotCategory)] =
            TUnitTestProperties.NotCategory;
    }
    
    public IEnumerable<TestCase> FilterTests(IEnumerable<TestCase> tests)
    {
        var filterExpression = runContext.GetTestCaseFilter(_supportedProperties.Keys, 
            propertyName => _supportedProperties.GetValueOrDefault(propertyName));

        messageLogger.SendMessage(TestMessageLevel.Informational, $"TestCaseFilterValue is: {filterExpression?.TestCaseFilterValue}");
        
        if (string.IsNullOrWhiteSpace(filterExpression?.TestCaseFilterValue))
        {
            foreach (var testWithTestCase in tests)
            {
                yield return testWithTestCase;
            }
            
            yield break;
        }
        
        var filter = ParseFilter(filterExpression.TestCaseFilterValue);
        
        foreach (var test in tests)
        {
            if (filterExpression.MatchTestCase(test, propertyName =>
                    !_supportedProperties.TryGetValue(propertyName, out var testProperty)
                        ? false
                        : test.GetPropertyValue(testProperty))
                && TestMatchesFilter(test, filter)
               )
            {
                yield return test;
            }
        }
    }

    private Filter ParseFilter(string testCaseFilterValue)
    {
        var filter = new Filter();
        
        foreach (var filterSegment in testCaseFilterValue.Split(';'))
        {
            var filterSplit = filterSegment.Split('=');
            var filterName = filterSplit.FirstOrDefault();
            var filterValue = filterSplit.ElementAtOrDefault(1);

            if (string.IsNullOrWhiteSpace(filterName) || 
                !TestAdapterConstants.Filters.KnownFilters.Contains(filterName, StringComparer.InvariantCultureIgnoreCase))
            {
                continue;
            }

            filter.AddFilter(filterName, filterValue);
        }

        return filter;
    }

    private bool TestMatchesFilter(TestCase test, Filter filter)
    {
        if (filter.IsEmpty)
        {
            return true;
        }

        if (filter.BannedCategories.Intersect(
                test.GetPropertyValue(TUnitTestProperties.Category, Array.Empty<string>())
            ).Any())
        {
            return false;
        }

        return AllowedTestName(test, filter)
               && AllowedCategory(test, filter)
               && AllowedClass(test, filter);
    }

    private static bool AllowedTestName(TestCase test, Filter filter)
    {
        return !filter.RunnableTestNames.Any() ||
               filter.RunnableTestNames.Contains(test.GetPropertyValue(TUnitTestProperties.TestName, "#"), StringComparer.InvariantCultureIgnoreCase);
    }
    
    private static bool AllowedCategory(TestCase test, Filter filter)
    {
        return !filter.RunnableCategories.Any() ||
               filter.RunnableCategories.Intersect(test.GetPropertyValue(TUnitTestProperties.Category, Array.Empty<string>()), StringComparer.InvariantCultureIgnoreCase).Any();
    }
    
    private static bool AllowedClass(TestCase test, Filter filter)
    {
        return AllowedSimpleClass(test, filter)
            && AllowedFullyQualifiedClass(test, filter);
    }
    
    private static bool AllowedSimpleClass(TestCase test, Filter filter)
    {
        return !filter.RunnableClasses.Any() ||
               filter.RunnableClasses.Contains(test.GetPropertyValue(TUnitTestProperties.TestClass, "#"), StringComparer.InvariantCultureIgnoreCase);
    }
    
    private static bool AllowedFullyQualifiedClass(TestCase test, Filter filter)
    {
        var className = test.GetPropertyValue(TUnitTestProperties.TestClass, "#");
        return !filter.RunnableFullyQualifiedClasses.Any() ||
               filter.RunnableFullyQualifiedClasses.Any(x => string.Equals(x, className, StringComparison.InvariantCultureIgnoreCase));
    }
}