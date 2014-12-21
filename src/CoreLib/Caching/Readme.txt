Notes:
1. Caching abstractions have been added to CoreLib.Caching: 
   - several interfaces
   - a cache decorator class that acts as a simple extensibility point for other developers
   - and some simple cache administrator functionality 
2. the real body of the code is to be found in a new Commons project named Commons.EntLibCaching. Classes in here augment 
   the  Microsoft Enterprise Library Caching block. The main class in Commons.EntLibCaching project is EntLibCache. This wraps an instance 
	of an ICacheManager from the Caching block. It takes an instance of a CachePolicy that defines such things as when items in the 
   ICacheManager should be expired (removed) from the cache
3. There is some other classes that deal with loading CachePolicy objects from xml configuration. These are found in Commons.App.Castle,
   and they are designed for those apps that use castle windsor (also in Commons.App) functionality to do dependency injection. 
   These classes are optional and are not required for those apps that do not want to take the dependency on castle windsor
4. For more information:
   a. See the tests/examples in Commons.UnitTests.Caching
   b. Install Enterprise library 4.1 if you haven't already and you get loads of documentation on the Enterprise library caching 
      classes and how to configure them
