# VisualScriptingKai

Previously known as VisualScriptingPlus (name changed due to overlap with existing Asset Store product).

Quality-of-life extensions to Unity Visual Scripting. Features not just a node search tool, but also other debugging tools as well. Visual Scripting Kai aims to give Unity Visual Scripting much-needed features to actually make it production-ready and competitive against well-known solutions like the Playmaker, and of course Unreal's Blueprint.

Live demo featuring a [non-trivial Visual Scripting project](https://github.com/chocola-mint/U1W_ShijiQuest):

https://github.com/chocola-mint/VisualScriptingKai/assets/56677134/a8523873-c5b5-4836-9e55-5f607c9865a6

(The project above has over 700 graph units, over 50 states, and over 40 state transitions, yet Visual Scripting Kai still performs extremely smoothly!)

Feel free to request additional features in the [Issues](https://github.com/chocola-mint/VisualScriptingKai/Issues) page.

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

https://github.com/chocola-mint/VisualScriptingKai/assets/56677134/4d032869-56ef-4838-b5bb-8cbb299d887b

(Events > VS Kai > Test)

A powerful debugging node that lets you run Visual Scripting code anytime, anywhere, by just clicking on the "Run" button. Use it as a way to test logic quickly.

### Function Nodes

(VSKai > Functions)

Functions are a more powerful and safer version of Events, defined using a Script Graph Asset, like this:

![Function Definition](https://github.com/chocola-mint/VisualScriptingKai/assets/56677134/1524a3d2-1d8a-4e5f-b6d0-9ea8728acdf3)


With the Function Start and Function Return nodes, you can implement a function that has the same ports as a Script Graph Asset (this asset is called the function definition). 

![Function Implementation](https://github.com/chocola-mint/VisualScriptingKai/assets/56677134/9dd8d9c5-3256-4980-91f9-3448b34e563b)

Other Script Machines can then use the Check Function or Has Function nodes to check if a GameObject implements a function, and if so, call it using the Call Function node.

![Function Check and Call](https://github.com/chocola-mint/VisualScriptingKai/assets/56677134/a205cdbf-38e0-4aff-ac18-f463f73ce782)


Every Function node must be assigned a function definition through the Graph Inspector (on your left).

![Assigning a Function Definition](https://github.com/chocola-mint/VisualScriptingKai/assets/56677134/460ed564-5160-4bed-bb51-27bae9f6f095)


This feature unlocks Visual Scripting's potential for object-oriented programming, similar to Unreal's Blueprint interfaces. For example, you can use it to check if a bullet is colliding with something that can take damage (e.g., the player) or not (e.g., the wall).

Notes:
- You cannot implement the same function twice on the same GameObject.
- A complete function implementation must include a Function Start and a Function Return, and they must be in the same Script Machine. The nodes Function Start and Function Return must be on the top level of the Script Machine's graph (if you put them inside Subgraphs, they will be ignored).
- Calling a function on a GameObject that doesn't implement it is undefined behavior. You should always use Check Function to check if a target implements a function first. Or use Has Function with an If node.

### Flow Coroutines


https://github.com/chocola-mint/VisualScriptingKai/assets/56677134/9317eed6-465a-4e84-bc5b-208a17b982e3


(VSKai > Coroutines)

Flow Coroutines are coroutines that can be launched from **any** node, and not just Events. The Start Flow Coroutine node can be used to launch a new Flow Coroutine. The logic of the Flow Coroutine is defined by the "Coroutine Start" port. You can also use the Flow Coroutine object returned by Start Flow Coroutine later, for example stopping it with the Stop Flow Coroutine node.

Flow Coroutines follow the same rules as any other coroutine - they belong to the GameObject they are spawned on, and will be stopped when the GameObject becomes inactive.

## Requirements

* This project is developed using Unity 2021.3.30f1, but should work with version 2021.3 and above in general.

## Installation

* Visual Scripting Kai is distributed as a git package. Use Unity's [Package Manager](https://docs.unity3d.com/Manual/upm-ui-giturl.html) and install using this repository's URL: `https://github.com/chocola-mint/VisualScriptingKai.git`
* After installing, go to `Project Settings > Visual Scripting` and click `Regenerate Nodes` to enable the Test Node and Function Nodes feature, if needed.

## Limitations

Support for embedded graphs inside scenes and prefabs is limited due to technical limitations. (For example, Graph Lens and the like can only detect them if they are currently open) Avoid them whenever possible to make the best use out of Visual Scripting Kai.
