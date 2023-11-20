# VisualScriptingPlus

Quality-of-life extensions to Unity Visual Scripting. Features not just a node search tool, but also other debugging tools as well. Visual Scripting Plus aims to give Unity Visual Scripting much-needed features to actually make it production-ready and competitive against well-known solutions like the Playmaker, and of course Unreal's Blueprint.

Feel free to request additional features in the [Issues](https://github.com/chocola-mint/VisualScriptingPlus/Issues) page.

## Features

### Graph Lens

A fuzzy search tool that can hunt down nodes and sticky notes in your graphs. Clicking the search results lets you jump straight to where the node is.

You can also use the sticky note search tool as a way to hunt down TODOs. This can be a powerful tool that lets you navigate through a large graph with ease.

### Graph Analyzer

A tool that can hunt down graph warnings. You can specify the minimum warning level to filter out unimportant warnings. Useful for cleaning up unused nodes and finding broken nodes quickly.

### Graph Debugger

A runtime tool that automatically pauses the game in Play Mode whenever a graph encounters an exception. The stack trace is laid out as well so you can quickly figure out why the exception happened. This can save you a lot of time by skipping the part where you have to dig through a long chain of red nodes to find the exception-causing node.

### Test Node

A powerful debugging node that lets you run Visual Scripting code anytime, anywhere, by just double-clicking on it. Use it as a way to test logic quickly.

## Requirements

* This project is developed using Unity 2021.3.30f1, but should work with version 2021.3 and above in general.

## Installation

* Visual Scripting Plus is distributed as a git package. Use Unity's [Package Manager](https://docs.unity3d.com/Manual/upm-ui-giturl.html) and install using this repository's URL: `https://github.com/chocola-mint/VisualScriptingPlus.git`
* After installing, go to `Project Settings > Visual Scripting` and click `Regenerate Nodes` to enable the Test Node feature, if needed.

## Limitations

Support for embedded graphs inside scenes and prefabs is limited due to technical limitations. (For example, Graph Lens and the like can only detect them if they are currently open) Avoid them whenever possible to make the best use out of Visual Scripting Plus.