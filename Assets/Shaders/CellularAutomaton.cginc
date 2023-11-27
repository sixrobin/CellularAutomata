float _Resolution;     // Automaton resolution (width, height, and depth for 3D automata).
int4 _Rules;           // Automaton ruleset (X = survive, Y = birth, Z = cell states count, W = neighbourhood).
float _InitRandomStep; // Chances of a cell to start the simulation alive.
int _InitCenterWidth;  // Maximum distance to automaton center for a cell to start the simulation alive.