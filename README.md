# 🕸️ Custom Graph Database (C#)

![Build Status](https://img.shields.io/badge/build-passing-brightgreen) ![.NET](https://img.shields.io/badge/.NET-8.0-purple) ![License](https://img.shields.io/badge/license-MIT-blue)

A powerful, typed graph database developed from scratch in C#. The project implements its own data structures and classic algorithms without using third-party graphics libraries.

## 🌟 Особливості

* **Generic Architecture:** Flexible type system `<TKey, TData, TEdgeData>`.
* **Polymorphism Support:**
    * Nodes: `PersonData`, `CityData`, `CompanyData`.
    * Edges: `FriendshipEdge`, `WorksAtEdge`.
* **Algorithms:**
    * 🔍 **BFS:**.
    * 🧠 **DFS:**.
    * ⚡ **Dijkstra:**.
    * 🔄 **Cycle Detection:**.
* **Persistence:** Saving/Loading a database in JSON (System.Text.Json).
* **Unit Testing:** Coverage with xUnit tests (20+ tests).

## 🏗️ Class Architecture

```mermaid
classDiagram
    class Graph {
        +Dictionary Nodes
        +AddNode()
        +AddEdge()
        +RemoveNode()
    }
    class Node {
        +TKey ID
        +TData Data
        +List Edges
    }
    class Edge {
        +Node From
        +Node To
        +TEdgeData Data
    }
    Graph *-- Node
    Node *-- Edge
