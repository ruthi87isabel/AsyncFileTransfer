## Async file transfer project

### **Solution description**

Platform: .Net Core 3.1

Language: C#

The solution (**AsyncFileTransfer.sln**) should be oppened in Visual Studio 2019 or a higher version. It contains mainly three projects:

- **AsyncFileTransfer**: .NET Core 3.1 Console Application project.
- **AsyncFileTransferProcessor**: .NET Core 3.1 Class Library project.
- **AsyncFileTransferProcessor.Tests**: .NET Core 3.1 NUnit Test project.

### **Running the application**

- Open the solution in Visual Studio 2019 or higher version (AsyncFileTransfer.sln)
- Set the **AsyncFileTransfer** console application as StartUp project
- Run (F5)

### **Commands**

In order to accomplish the requirements of the technical task, two commands has been implemented:

- transfer -s "source-folder-path" -d "target-folder-path"
- exit

The **transfer** command receives two parameters: the source and the target folder paths. The application will list and copy all the files from the source folder to the target folder as described in the requirements.

The parameters can be espeficied in any of the following formats:

- transfer -s "source-folder-path" -d "target-folder-path"
- transfer --src "source-folder-path" --dest "target-folder-path"

The **exit** command has no parameters. Use this command to stop the application and dispose all the resources used (threads for instance).

Any other command will be unknown and therefore the console will display an error message.
