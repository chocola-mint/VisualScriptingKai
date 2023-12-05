# VisualScriptingPlus


Quality-of-life extensions to Unity Visual Scripting. Features not just a node search tool, but also other debugging tools as well. Visual Scripting Plus aims to give Unity Visual Scripting much-needed features to actually make it production-ready and competitive against well-known solutions like the Playmaker, and of course Unreal's Blueprint.

Live demo featuring a [non-trivial Visual Scripting project](https://github.com/chocola-mint/U1W_ShijiQuest):

https://github.com/chocola-mint/VisualScriptingPlus/assets/56677134/a8523873-c5b5-4836-9e55-5f607c9865a6

(The project above has over 700 graph units, over 50 states, and over 40 state transitions, yet Visual Scripting Plus still performs extremely smoothly!)

Feel free to request additional features in the [Issues](https://github.com/chocola-mint/VisualScriptingPlus/Issues) page.

## Features

### Graph Lens

(Window > Visual Scripting > Graph Lens)

A fuzzy search tool that can hunt down nodes, sticky notes, states, and state transitions in your graphs. Clicking the search results lets you jump straight to where the node is.

You can also use the sticky note search tool as a way to hunt down TODOs. This can be a powerful tool that lets you navigate through a large graph with ease.

Note that for elements that can have their names changed manually (Sticky Notes and State Transitions for example), they won't be reflected right away in the search results. Save the graph with Ctrl+S if you want to see it quickly.

### Graph Analyzer

(Window > Visual Scripting > Graph Analyzer)

A tool that can hunt down graph warnings. You can specify the minimum warning level to filter out unimportant warnings. Useful for cleaning up unused nodes and finding broken nodes quickly.

### Graph Debugger

(Window > Visual Scripting > Graph Debugger)

A runtime tool that automatically pauses the game in Play Mode whenever a graph encounters an exception. The stack trace is laid out as well so you can quickly figure out why the exception happened. This can save you a lot of time by skipping the part where you have to dig through a long chain of red nodes to find the exception-causing node.

### Test Node

https://github.com/chocola-mint/VisualScriptingPlus/assets/56677134/95662b4f-e4ae-4c6d-9547-011eb86e842a

(Events > VS Plus > Test)

A powerful debugging node that lets you run Visual Scripting code anytime, anywhere, by just double-clicking on it. Use it as a way to test logic quickly.

### Functions

Functions are a more powerful and safer version of Events, defined using a Script Graph Asset. With the Function Start and Function Return nodes, you can implement a function that has the same ports as a Script Graph Asset (this asset is called the function definition). Other Script Machines can then use the Check Function or Has Function nodes to check if a GameObject implements a function, and if so, call it using the Call Function node.

Every Function node must be assigned a function definition through the Graph Inspector (on your left).

This feature unlocks Visual Scripting's potential for object-oriented programming, similar to Unreal's Blueprint interfaces. For example, you can use it to check if a bullet is colliding with something that can take damage (e.g., the player) or not (e.g., the wall).

Notes:
- You cannot implement the same function twice on the same GameObject.
- A complete function implementation must include a Function Start and a Function Return, and they must be in the same Script Machine. The nodes Function Start and Function Return must be on the top level of the Script Machine's graph (if you put them inside Subgraphs, they will be ignored).
- Calling a function on a GameObject that doesn't implement it is undefined behavior. You should always use Check Function to check if a target implements a function first. Or use Has Function with an If node.

## Requirements

* This project is developed using Unity 2021.3.30f1, but should work with version 2021.3 and above in general.

## Installation

* Visual Scripting Plus is distributed as a git package. Use Unity's [Package Manager](https://docs.unity3d.com/Manual/upm-ui-giturl.html) and install using this repository's URL: `https://github.com/chocola-mint/VisualScriptingPlus.git`
* After installing, go to `Project Settings > Visual Scripting` and click `Regenerate Nodes` to enable the Test Node feature, if needed.

## Limitations

Support for embedded graphs inside scenes and prefabs is limited due to technical limitations. (For example, Graph Lens and the like can only detect them if they are currently open) Avoid them whenever possible to make the best use out of Visual Scripting Plus.
