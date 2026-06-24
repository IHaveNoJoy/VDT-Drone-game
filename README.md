# AR Drone Game - Dronecup

**Primary Developers:** Tom van de Lest, Vince van Gestel  
**Repository:** [VDT-Drone-game](https://github.com/IHaveNoJoy/VDT-Drone-game.git)  
**Target Event:** Dronecup  

---

## 🎯 Project Objectives & Scope
The core objective of this project is to deliver a highly engaging, short-duration experience integrating physical drone flights with digital gameplay. Built specifically for the Dronecup, the game emphasizes rapid turnover times, robust tracking, and intuitive interactions for quick onboarding during the event.

## 💻 Technical Stack

| Technology | Version / Details | Implementation Purpose |

| **Unity Engine** | Unity 6 | Primary rendering engine, physics simulation, and AR foundation management. |
| **Programming Language**| C# | Game logic, state management, and hardware API integration. |
| **Version Control** | Git / GitHub | Source code management and asset tracking. |

## ⚙️ Core Systems & Integration

### Eindhoven Drone Control Integration
A crucial technical pillar of this application is the communication bridge between the physical drone hardware and the digital Unity environment. We utilize specialized bonus files provided by students from Eindhoven.
* **Telemetry Translation:** The files capture real-time positional data and translate it into a readable format for the Unity engine.
* **Input Handling:** The custom scripts intercept the control signals, ensuring the digital AR elements remain synchronized with the physical drone's flight path.

## 🚀 Installation & Setup Guide

To initialize the development environment on a new machine, follow these precise steps to ensure all dependencies are correctly linked.

1. **Clone the Repository**  
   Ensure Git is installed and execute the following command in your terminal:
bash
   git clone [https://github.com/IHaveNoJoy/VDT-Drone-game.git](https://github.com/IHaveNoJoy/VDT-Drone-game.git)