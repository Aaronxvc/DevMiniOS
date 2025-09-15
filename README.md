# DevMiniOS - Offline DSL IDE

DevMiniOS is a lightweight, offline-first Integrated Development Environment (IDE) designed for creating and experimenting with Domain-Specific Languages (DSLs). It provides a natural language interface for generating code, managing projects, and integrating with Git.

## Key Features

*   **Natural Language to Code Generation:**  Translate natural language prompts into structured commands and executable code using a rule-based planning system.

*   **Offline Operation:**  Develop and generate code without relying on external network connections or third-party services.

*   **Template-Driven Code Generation:**  Generate code in multiple languages (C#, C++, JavaScript) from predefined templates.

*   **Integrated Git Operations:**  Initialize Git repositories, add changes, commit code, and push/pull from remote repositories directly from the IDE (requires Git command-line tools).

*   **Mini-LM for Intelligent Code Completion:** A small, local language model provides context-aware code completion suggestions for DSLs, enhancing productivity.

## Architecture

[**Include a diagram here showing the system architecture.** Example:  MVC App <=> C# API <=> Python Mini-LM (Optional C++)]

DevMiniOS follows a modular architecture:

1.  **MVC Front-End:** Provides the user interface for interacting with the IDE.

2.  **C# Web API:** Acts as a bridge between the front-end and the core code generation and Git services.  Handles requests and provides responses.

3.  **Rule-Based Planner:** Analyzes natural language input and generates a structured plan of actions.

4.  **Code Generator:** Uses templates and the generated plan to produce source code in the desired language.

5.  **Git Service:** Executes Git commands using the command-line interface.

6.  **Mini-LM (Python):** Provides intelligent code completion suggestions based on a small, local language model.

## Mini-LM Details

*   Implemented using a bigram language model in Python.
*   Trained on a corpus of DSL commands and code snippets.
*   Provides code completion suggestions based on the preceding words.

## Getting Started

1.  **Prerequisites:**

    *   .NET 9 SDK
    *   Python 3.x (for the mini-LM)
    *   Git (for Git integration)

2.  **Clone the repository:**
    ```bash
    git clone <repository_url>
    ```

3.  **Build the application:**
    ```bash
    dotnet build
    ```

4.  **Run the application:**
    ```bash
    dotnet run
    ```

5.  **Access the IDE in your browser:**  Navigate to `https://localhost:<port>/` (replace `<port>` with the port number shown in the console).

## Usage

1.  Enter a natural language prompt in the input field.

2.  Select the target programming language.

3.  Click the "Generate" button.

4.  The IDE will display the generated code and DSL summary.

## Configuration

*   **Python Path:** Configure the path to the Python executable in `appsettings.json` using the `MiniModel:PythonPath` key.

*   **Model Location:** The mini-LM model is stored in the `MiniModel/out/model.json` file.

## Contributing

[Explain how others can contribute to your project.]

## License

[Specify the license under which your project is distributed.]
