## Contributing to Rise Media Player

### Contributing guidelines
Contributing anytime is always welcome, but

1. We recommend you to look at the Issues tab on the repository and check the issues with labels like "triage approved", "help wanted", these are usually possible to do and verified by our team of collaborators. You are also welcome to contribute fixes/features that are not with these labels, but those will have a slightly lower priority than those with the labels.
2. In the code (XAML or C#), use proper padding, styles as per [Microsoft's C# coding conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions). You can use a code formatter if you want :)
3. If you are developing a new feature or fixing a bug, always check if there's an existing user control, extension method for the function you're looking for before adding a new method, and add it to a helper class if you feel like the code is going to be reused in multiple files.
4. Be familiar with the Model-View-View-Model (MVVM) pattern, it's used in most parts of the app and makes things easier to maintain in the meantime.

### Guidelines for issues
1. Make sure the issue is reproducible after many attempts, and if the bug persists even after restart.
2. Label your issue correctly, and don't add labels like "triage approved" and "help wanted" because these are usually handled by the team.
3. Add screenshots/videos/mockups of your issue so that it becomes clearer.
4. Properly describe the bug/feature you are talking about, failing to do so for bugs might result in your issue being closed.
5. Add your Windows version and build number you're currently running RiseMP on, for example: Windows 11 build 22000.318, Windows 10 build 18362.1 etc...
