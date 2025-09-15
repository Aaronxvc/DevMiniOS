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

We welcome contributions to DevMiniOS! Whether you're a seasoned developer, a documentation enthusiast, or just starting out, there are many ways you can help.

### How to Contribute

1.  **Fork the repository:** Click the "Fork" button in the upper-right corner of the GitHub repository to create your own copy.

2.  **Clone your fork:** Clone your forked repository to your local machine:
    ```bash
    git clone https://github.com/your-username/DevMiniOS.git
    ```

3.  **Create a branch:** Create a new branch for your changes:
    ```bash
    git checkout -b feature/your-feature-name
    ```
    *   Use descriptive branch names, such as `feature/add-new-dsl-command` or `fix/typo-in-readme`.

4.  **Make your changes:** Implement your desired changes, following the coding standards outlined below.

5.  **Commit your changes:** Commit your changes with a clear and concise commit message:
    ```bash
    git commit -m "Add new DSL command for database interaction"
    ```

6.  **Push your changes to your fork:**
    ```bash
    git push origin feature/your-feature-name
    ```

7.  **Submit a pull request:** Go to your forked repository on GitHub and click the "Create pull request" button.

### Types of Contributions

*   **Code Contributions:**
    *   Implement new features or DSL commands.
    *   Fix bugs.
    *   Improve performance.
    *   Add tests.
*   **Documentation Contributions:**
    *   Improve the README file.
    *   Add or update API documentation.
    *   Create tutorials or examples.
*   **Bug Reports:** Report any bugs or issues you encounter by creating a new issue in the GitHub issue tracker.
*   **Feature Requests:** Suggest new features or improvements by creating a new issue in the GitHub issue tracker.

### Coding Standards

*   Follow the existing coding style and naming conventions.
*   Add XML documentation comments to all new classes, methods, and properties in C#.
*   Add docstrings to all new functions and classes in Python.
*   Write unit tests for all new code.
*   Keep code changes small and focused.
*   Ensure that your changes don't introduce any new warnings or errors.

### Issue Tracking

Use the GitHub issue tracker to report bugs, suggest new features, or ask questions.

### Code of Conduct

We are committed to creating a welcoming and inclusive community. Please review our [Code of Conduct](link-to-code-of-conduct.md) before contributing.

Thank you for your contributions!

## License

MIT License 
