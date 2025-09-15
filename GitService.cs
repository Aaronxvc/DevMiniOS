// Path: Services/GitService.cs
using System.Diagnostics;

namespace DevMiniOS.Services;

/// <summary>
/// Provides Git integration using the command-line interface.
/// </summary>
public class GitService
{
    private readonly string _projectRoot;
    private readonly string? _gitCloneUrl; // Optional Git clone URL

    /// <summary>
    /// Initializes a new instance of the <see cref="GitService"/> class.
    /// </summary>
    /// <param name="projectRoot">The root directory of the project.</param>
    /// <param name="gitCloneUrl">Optional URL to clone a Git repository from.</param>
    public GitService(string projectRoot, string? gitCloneUrl = null)
    {
        _projectRoot = projectRoot;
        _gitCloneUrl = gitCloneUrl;

        // Attempt to clone the repository if a URL is provided.
        if (!string.IsNullOrEmpty(_gitCloneUrl))
        {
            CloneRepository(_gitCloneUrl, _projectRoot);
        }
    }

    /// <summary>
    /// Clones a Git repository from the specified URL to the specified directory.
    /// </summary>
    /// <param name="url">The URL of the Git repository to clone.</param>
    /// <param name="directory">The directory to clone the repository into.</param>
    /// <returns><c>true</c> if the clone was successful, <c>false</c> otherwise.</returns>
    public bool CloneRepository(string url, string directory)
    {
        if (!IsGitInstalled())
        {
            Console.WriteLine("Git is not installed. Please install Git to use Git features.");
            return false;
        }

        //Check if directory is already initialized
        if (Directory.Exists(Path.Combine(directory, ".git")))
        {
            Console.WriteLine($"Directory {directory} is already initialized");
            return true;
        }

        Console.WriteLine($"Cloning from {url} to {directory}");
        return ExecuteGitCommand($"clone \"{url}\" \"{directory}\"") == 0;
    }

    /// <summary>
    /// Initializes a Git repository in the project root if one doesn't exist.
    /// </summary>
    /// <returns><c>true</c> if the repository was initialized, <c>false</c> otherwise.</returns>
    public bool InitializeRepository()
    {
        if (!IsGitInstalled())
        {
            Console.WriteLine("Git is not installed. Please install Git to use Git features.");
            return false;
        }

        if (Directory.Exists(Path.Combine(_projectRoot, ".git")))
        {
            Console.WriteLine("Git repository already initialized.");
            return true;
        }

        return ExecuteGitCommand("init") == 0;
    }

    /// <summary>
    /// Adds all changes in the project root to the Git staging area.
    /// </summary>
    /// <returns><c>true</c> if the operation was successful, <c>false</c> otherwise.</returns>
    public bool AddChanges()
    {
        return ExecuteGitCommand("add .") == 0;
    }

    /// <summary>
    /// Commits the staged changes with the specified message.
    /// </summary>
    /// <param name="message">The commit message.</param>
    /// <returns><c>true</c> if the commit was successful, <c>false</c> otherwise.</returns>
    public bool CommitChanges(string message)
    {
        return ExecuteGitCommand($"commit -m \"{message}\"") == 0;
    }

    /// <summary>
    /// Adds a remote repository.
    /// </summary>
    /// <param name="name">The name of the remote.</param>
    /// <param name="url">The URL of the remote repository.</param>
    /// <returns><c>true</c> if the operation was successful, <c>false</c> otherwise.</returns>
    public bool AddRemote(string name, string url)
    {
        return ExecuteGitCommand($"remote add {name} {url}") == 0;
    }

    /// <summary>
    /// Pushes changes to the specified remote.
    /// </summary>
    /// <param name="remote">The remote to push to.</param>
    /// <param name="branch">The branch to push.</param>
    /// <returns><c>true</c> if the push was successful, <c>false</c> otherwise.</returns>
    public bool PushChanges(string remote, string branch)
    {
        return ExecuteGitCommand($"push {remote} {branch}") == 0;
    }

    /// <summary>
    /// Pulls changes from the specified remote.
    /// </summary>
    /// <param name="remote">The remote to pull from.</param>
    /// <param name="branch">The branch to pull.</param>
    /// <returns><c>true</c> if the pull was successful, <c>false</c> otherwise.</returns>
    public bool PullChanges(string remote, string branch)
    {
        return ExecuteGitCommand($"pull {remote} {branch}") == 0;
    }

    private bool IsGitInstalled()
    {
        try
        {
            ProcessStartInfo psi = new("git", "--version")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(psi)!)
            {
                process.WaitForExit();
                return process.ExitCode == 0;
            }
        }
        catch
        {
            return false;
        }
    }

    private int ExecuteGitCommand(string command)
    {
        if (!IsGitInstalled())
        {
            Console.WriteLine("Git is not installed. Please install Git to use Git features.");
            return -1; // Indicate failure
        }
        ProcessStartInfo psi = new("git", command)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true, // Capture error stream
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = _projectRoot // Set the working directory
        };

        try
        {
            using (Process process = Process.Start(psi)!)
            {
                process.WaitForExit();

                // Optionally, log the output and error streams for debugging
                Console.WriteLine(process.StandardOutput.ReadToEnd());
                Console.Error.WriteLine(process.StandardError.ReadToEnd()); // Log errors

                return process.ExitCode;
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error executing git command: {ex.Message}");
            return -1; // Indicate failure
        }
    }
}