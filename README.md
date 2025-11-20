# Custom Graph Database (C#)

A lightweight graph database implemented from scratch in C# without using third-party libraries for data structures. The project demonstrates a deep understanding of algorithms, data structures, and OOP principles.

## 🚀 Main features

* **Generics:** Fully typed structure (Key, Node data, Edge data).
* **Polymorphism:**
    * Support for different types of nodes (`Person`, `City`, `Company`).
    * Different types of edges (`Friendship`, `WorksAt`).
* **Algorithms:**
    * 🔍 **BFS**.
    * 🧠 **DFS**.
    * ⚡ **Dijkstra**.
    * 🔄 **Cycle Detection**
* **Persistence:** Saving and loading databases in JSON format.

## 🛠️ Technologies

* **Language:** C# (.NET 9.0)
* **Testing:** xUnit
* **Serialization:** System.Text.Json
* **Patterns:** MVC (Console UI separate from logic), DTO, Repository (Graph).
