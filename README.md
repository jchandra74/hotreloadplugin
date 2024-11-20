# Hot Reload Plugin Example in .NET
This example illustrate how to create a hot reload plugin system using .NET

This example is implemented in .NET 9.0, so you'll need to install that. You can get it from [https://dot.net]

The example constains multiple projects
```
src
 - app (the main console application)
 - core (contains the IWidget interface)
 - hello (a plugin implementation of IWidget)
 - goodbye (a plugin implementation of IWidget)
 ```

 To test, build each projects using `dotnet build` and start running `app.exe` from the `bin\Debug\net9.0` directory.
 
 The application will start a loop until you press `Ctrl+C`.

 Now, copy `src\hello\bin\Debug\net9.0\hello.dll` into `src\app\bin\Debug\net9.0\Plugins` folder.

 You should see `Hello, World!` in the console for every 1 second.

 Copy `src\goodbye\bin\Debug\net9.0\goodbye.dll` into `src\app\bin\Debug\net9.0\Plugins` folder.

 You should also see `Goodbye, World!` in the console for every 1 second.

 Delete `src\app\bin\Debug\net9.0\Plugins\hello.dll`.

 You should no longer see `Hello, World` in the console on each tick.