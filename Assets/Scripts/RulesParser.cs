namespace CellularAutomata
{
    using UnityEngine;

    public static class RulesParser
    {
        private const char RULES_SEPARATOR = '/';
        private const char NEIGHBOUR_RULES_SEPARATOR = ',';
        private const char MIN_MAX_NEIGHBOURS_COUNT_SEPARATOR = '-';
        
        private const string MOORE_NEIGHBOURHOOD_ID = "M";
        private const string MOORE_NEIGHBOURHOOD_NAME = "Moore";
        private const string VON_NEUMANN_NEIGHBOURHOOD_ID = "N";
        private const string VON_NEUMANN_NEIGHBOURHOOD_NAME = "VonNeumann";

        private const int CELL_MINIMUM_STATES_COUNT = 2;
        
        /// <summary>
        /// Parses the birth and survive rules of a cellular automaton.
        /// The rule uses the format A/B/C/D where A is the survive rule, B the birth rule, C the number of states a cell can have, and D the neighbourhood index.
        /// Each neighbours count is split by a comma (,), and ranges can be marked using a hyphen (-). For instance, 1-3,7,8 means valid neighbours counts are 1, 2, 3, 7 and 8.
        /// The states count has a minimum of 2 and can then take as many values as desired.
        /// The neighbourhood is either 0 for Moore, or 1 for Von Neumann.
        /// Returns a Vector2Int. Each birth neighbours count is converted using 2^n formula and added to the x value. Same for survive neighbours count with y value.
        /// To check a cell, simply count its neighbours, convert it using the same 2^n formula and compare it to the rule using a AND operator.
        /// </summary>
        /// <param name="rules">Ruleset string (A/B/C/D format where C and D are optional).</param>
        /// <param name="log">Logs the parsed ruleset to Unity console.</param>
        /// <returns>Rules as a 4 ints Tuple format.</returns>
        public static (int, int, int, int) ParseRuleset(string rules, bool log = false)
        {
            try
            {
                rules = rules.ToUpper();

                // Split birth and survive rules.
                string[] rulesSplit = rules.Split(RULES_SEPARATOR);

                int surviveRule = ParseNeighboursRule(rulesSplit[0]);
                int birthRule = ParseNeighboursRule(rulesSplit[1]);
                int cellStatesRule = rulesSplit.Length >= 3 ? ParseCellStatesRule(rulesSplit[2]) : 2;
                int neighbourhoodRule = rulesSplit.Length >= 4 ? ParseNeighbourhoodRule(rulesSplit[3]) : 0;

                if (log)
                {
                    Debug.Log($"Survive: {rulesSplit[0]} ({surviveRule})"
                              + $"\nBirth: {rulesSplit[1]} ({birthRule})"
                              + $"\nStates: {cellStatesRule}"
                              + $"\nNeighbourhood: {NeighbourhoodToString(neighbourhoodRule)}");
                }
                
                return (surviveRule, birthRule, cellStatesRule, neighbourhoodRule);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Could not parse rules {rules}! Make sure rules format is either A/B, A/B/C or A/B/C/D.\nException message: {e}");
                return (0, 0, 0, 0);
            }
        }

        /// <summary>
        /// Parses a neighbours counts rule.
        /// Each neighbours count is split by a comma (,), and ranges can be marked using a hyphen (-). For instance, 1-3,7,8 means valid neighbours counts are 1, 2, 3, 7 and 8.
        /// Returns an int. Neighbours count is the sum of each value converted using 2^n formula.
        /// </summary>
        /// <param name="rule">Rule to parse.</param>
        /// <returns>Parsed rule as an int.</returns>
        private static int ParseNeighboursRule(string rule)
        {
            if (rule.Length == 0)
                return -1; // -1 means no rule has been defined.
            
            string[] neighbourRules = rule.Split(NEIGHBOUR_RULES_SEPARATOR);
            int ruleToInt = 0;

            void ParseMinMaxValues(string value)
            {
                string[] minMaxValues = value.Split(MIN_MAX_NEIGHBOURS_COUNT_SEPARATOR);

                // Parse min value.
                if (!int.TryParse(minMaxValues[0], out int minValue))
                {
                    Debug.LogError($"Could not parse minValue {minMaxValues[0]} to a valid int!");
                    return;
                }

                // Parse max value.
                if (!int.TryParse(minMaxValues[1], out int maxValue))
                {
                    Debug.LogError($"Could not parse maxValue {minMaxValues[1]} to a valid int!");
                    return;
                }
                    
                // Loop from min value to max value and add neighbours count each iteration.
                for (int i = minValue; i <= maxValue; ++i)
                {
                    int neighboursCount = (int)Mathf.Pow(2, i); // Convert value to 2^value.
                    ruleToInt += neighboursCount; // Add value to birth rule.
                }
            }
            
            void ParseSingleValue(string value)
            {
                if (!int.TryParse(value, out int neighboursCount))
                {
                    Debug.LogError($"Could not parse neighbours count {value} to a valid int!");
                    return;
                }
                    
                neighboursCount = (int)Mathf.Pow(2, neighboursCount); // Convert value to 2^value.
                ruleToInt += neighboursCount; // Add value to birth rule.
            }
            
            foreach (string neighbourRule in neighbourRules)
            {
                if (neighbourRule.Contains(MIN_MAX_NEIGHBOURS_COUNT_SEPARATOR))
                    ParseMinMaxValues(neighbourRule);
                else
                    ParseSingleValue(neighbourRule);
            }

            return ruleToInt;
        }

        /// <summary>
        /// Parses the cell states rule.
        /// This methods does a single int parsing, and clamped the result to a minimum of 2 states.
        /// </summary>
        /// <param name="rule">Rule to parse.</param>
        /// <returns>Cells states count.</returns>
        private static int ParseCellStatesRule(string rule)
        {
            return int.TryParse(rule, out int ruleToInt) ? Mathf.Max(CELL_MINIMUM_STATES_COUNT, ruleToInt) : CELL_MINIMUM_STATES_COUNT;
        }

        /// <summary>
        /// Parses the neighbourhood rule.
        /// Valid neighbourhoods are "M" (Moore) and "N" (Von Neumann). Any other value will return a Moore neighbourhood.
        /// </summary>
        /// <param name="rule">Rule to parse.</param>
        /// <returns>Neighbourhood index (M=0, N=1).</returns>
        private static int ParseNeighbourhoodRule(string rule)
        {
            return rule.ToUpper() switch
            {
                MOORE_NEIGHBOURHOOD_ID => 0,
                VON_NEUMANN_NEIGHBOURHOOD_ID => 1,
                _ => 0,
            };
        }

        /// <summary>
        /// Converts a neighbourhood index to a string that can be used in UI or logs.
        /// </summary>
        /// <param name="neighbourhood">Neighbourhood index (M=0, N=1).</param>
        /// <returns>Neighbourhood name.</returns>
        private static string NeighbourhoodToString(int neighbourhood)
        {
            return neighbourhood switch
            {
                0 => MOORE_NEIGHBOURHOOD_NAME,
                1 => VON_NEUMANN_NEIGHBOURHOOD_NAME,
                _ => MOORE_NEIGHBOURHOOD_NAME,
            };
        }
    }
}