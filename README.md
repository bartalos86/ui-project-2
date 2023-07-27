# Genetic pathfinding algorithm
As an assignment we had to implement a genetic algorthm which is capable of finding the most amount of treasures on the map with the minimal path. 

## How does it work
The base of the algorithm are virtual machines which have predefined number of "genes", these genes are instructions which are one of the four possible instruction types: 
- INCREMET = 0,
- DECREMENT = 1,
- JUMP = 2,
- PRINT = 3

When one instruction is executed it can affect other genes, for example INCREMENT will increment the gene's value which it is pointing to, same with the DECREMENT, JUMP will jump to the target gene and execute it's instruction. The PRINT instruction is the final instruction in the execution of a virtual machine, when a PRINT instruction is reached the values which were generated during the execution of the virtual machine are read and executed as steps.

Each machine is rated after it finishes executing it's steps. Then the most successful machine's genes are mutated and crossed over.
This implementation includes ELITISM, which means the most successful machines automatically go into the next generation without being crossed over with other machines.

The program features different selection and crossover types which the user can choose as he likes:

### Selection types:
- Roulette selection
- Tournament selection

### Crossover types:
- Crossover (whole part of the genome is crossed over)
- Gene crossover (signle genes are crossed over)

## Input
The program is capable of reading the input from .txt files where the map should be defined in the following format:

```
size;7;7
start;3;6
gold;4;1
gold;2;2
gold;6;3
gold;1;4
gold;4;5
```

The first line is always the size of the map, the second line will be the starting position of the AIs, after that desired amount of tresures can be defined.

### How to run the program
- in `Program.cs` configure the map's location (it's hardcoded)
- `dotnet run`
- set the required prameters in terminal
- leave the algorithm do its thing :)

<img src="/Showcase/result.png">


