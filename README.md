# homemade-di

This project is a demo for a do-it-yourself dependency injection framework for `dotnet`.
I've written this for educational purpose.

## Purpose of this

Once upon a time, a junior developer asked himself:
"What is this dependency injection thing that all the seniors are talking about?"
He know how to use it, but he was not familiar with the principles behind it,
nor he fully understood why all of them are using it.

Once, I've been this developer, but now I'm one of the seniors who believe that dependency injection (DI)
is a very important and useful concept we all should be familiar with.
Then I sometimes thought how hard could it be to create a DI myself?
How hard could it be to create some instances of classes which are registered at first?
It's not magic, neither is it hard.
I'd like to show you now how to do it, maybe it helps you for your understanding.

## Why Dependency Injection?

Obviously, we do not implement DI as an end in itself.
There is a problem we can solve with DI and it's solution is called
**Dependency Inversion Principle** (the D of SOLI**D**, DIP abbreviated).

Assume an implementation like the following:
```c#
public class PeopleService
{
    private readonly PeopleRepository _peopleRepository;

    public PeopleService()
    {
        _peopleRepository = new PeopleRepository();
    }
}
```

The problem here is the creation dependency (`new`) to `PeopleRepository` (hard reference).

![Hard and direct (bad) dependency between classes](docs/hard-dependency.png)

*Hard and direct (bad) dependency between classes*

Therefore a hard coupling between `PeopleService` and `PeopleRepository` is created.
Whenever a PeopleService gets created, also a PeopleRepository get created.
Exactly this PeopleRepository is instantiated and there is no change to supply another implementation.
Hard coupling, as mentioned before.

We don't want this due to various reasons.
First of all, it's bad for code quality, as the quality attributes `testability` and `changeability`
cannot be met.

As a result, we cannot provide a mock instance to this `PeopleService` in order to write a unit-test for it.
The solution to this problem is to pull the creation dependency (the `new`) up, to the user
of the `PeopleService`.

![The dependency is now inversed, it's the responsibility of whoever wants to use the
PeopleService to pass an instance of IPeopleRepository](docs/inversed-dependency.png)

*The dependency is now inversed, it's the responsibility of whoever wants to use the
PeopleService to pass an instance of IPeopleRepository*

It's now his responsibility to supply a correct instance of `IPeopleRepository` to the `PeopleService`.

```c#
public class PeopleService
{
    private readonly PeopleRepository _peopleRepository;

    public PeopleService(IPeopleRepository peopleRepository)
    {
        _peopleRepository = peopleRepository;
    }
}
```

As you can infer from the naming, we'll use an interface type here as dependency,
in order to allow also supplying other implementations (e.g. for testing)
to `PeopleService` to work with.

```c#
var peopleService = new PeopleService(new PeopleRepository());
// or with another (mock) implementation
var peopleService = new PeopleService(new MockPeopleRepository());
```

## Dependency Injection basics

Imagine you apply the **Dependency Inversion Principle** everywhere across your whole app.
At some point, all the instances of your classes have to be instantiated and passed to the dependent classes.

![A dependency tree](docs/dependency-tree.png)

*A dependency tree*

You need A and B to create a PeopleService, but in order to create `A` you need `A1` and `A2`, and so on.
As you can see in the above graphic, a dependency tree arises.
Therefore, you have to start at the bottom where no dependencies are required (`A11` and `A12` here)
and work your way up, until you got a `PeopleService`.


If something is added, removed or moved inside this tree, you have to adjust it yourself.
This is especially impractical in large-scale applications with hundreds of classes.

This is where dependency injection comes in. It's a technique which creates these instances for you and
copes with all it's dependencies. The only thing you'd like to do is to have a small library
which you can call: I'd like to have an instance of `PeopleService`, could you give me some?

```c#
var peopleService = serviceLibrary.GetService<PeopleService>();
```

Still assuming we use interfaces as constructor parameters (to be more flexible, you remember)
we have to tell the DI framework which implementations should be used behind this interfaces.
We'll have to register in the first place which implementation should be used for which interface.

This will result in a table-like structure with a mapping from the interface-type to the implementation-type.

## Implementation

There are two different use-cases for a DI framework to cover:

* mapping a interface-implementation type pair
* resolving a service instance from it

I'll call my dependency injection framework `ServiceLibrary`.
Only a single class will be used, since it does not get too much code.
In consequence all the code snippets below are inside the `ServiceLibrary` class.

### Mapping types

We'll start off with the easy part.
As you can see below, the basic implementation is very easy.

```c#
private readonly Dictionary<Type, Type> _mappings = new();

public void Map(Type interfaceType, Type implementationType)
{
    _mappings[interfaceType] = implementationType;
}

// generic method for easier use
public void Map<TInterface, TImpl>()
    where TImpl : TInterface
{
    var interfaceType = typeof(TInterface);
    var implementationType = typeof(TImpl);

    Map(interfaceType, implementationType);
}
```

Of course additional checks can be added to this snippet, for example:
* Is this interface already registered?
* Does the implementation type even implement this interface?


### Resolving mapped types

When you'd like to retrieve an instance of a specific interface type,
a lookup based on the `_mappings` has to be made.
Depending on the concrete implementation type, all the dependencies for it
also have to be evaluated and built.
The above explanation is summarized in the following graphic:

![How the service resolve works with interfaces and mappings](docs/getservice-summary.png)

*How the service resolve works with interfaces and mappings*


This is possible by using reflection, since all dependencies are listed inside
the constructor and C# allows us to scan the constructor definition of any type.

```c#
private Type[] GetDependentTypes(Type implType)
{
    // retrieve constructor info
    var ctorInfo = implType.GetConstructors()
        .First();

    var parameterInfos = ctorInfo.GetParameters();

    if (parameterInfos.Length == 0)
    {
        return Array.Empty<Type>();
    }

    // only return the types of the parameters
    return parameterInfos.Select(x => x.ParameterType)
        .ToArray();
}
```

The below snippet shows how the creation of the instances is done.
It's implemented in a recursive manner because of the tree-structure which
may result depending on which types have to be resolved.

```c#
public object GetService(Type type)
{
    // lookup in the registered mappings
    var implType = _mappings[type];
    var dependentTypes = GetDependentTypes(implType);

    if (dependentTypes.Length == 0)
    {
        // no dependencies for this type (parameterless constructor)
        return Activator.CreateInstance(implType);
    }

    // create all dependencies to create this service
    var dependentInstances = new object[dependentTypes.Length];

    for (var i = 0; i < dependentTypes.Length; i++)
    {
        // traverse the tree with recursion
        dependentInstances[i] = GetService(dependentTypes[i]);
    }

    return Activator.CreateInstance(implType, dependentInstances);
}
```

I left some null-handling and edge-cases here, but basically that's it.

You can now add an generic method to make the library more easy to use:

```c#
public T GetService<T>()
{
    return (T) GetService(typeof(T));
}
```


## Usage

The implementation is now done, it's time to use it.
We'll use the generic methods we created, since this is easier in my opinion.
We just have to register all our interface-implementation type mappings first,
and afterwards we are able to resolve an interface from it.

```c#
var sl = new ServiceLibrary();

// register
sl.Map<IPeopleService, PeopleService>();
sl.Map<ITestDependencyX, TestDependencyX>();
sl.Map<ITestDependencyY, TestDependencyY>();

// resolve
var peopleService = sl.GetService<IPeopleService>();
```

## Conclusion

I haven't thought implementing a fully functional DI framework would be so easy upfront.
But the more I thought about it, the easier it got.
Nevertheless there are more use-cases which can be covered with DI,
for example service lifetime management (singletons) just to name one.

I hope the concept of dependency injection is clear to you now.
Feel free to look up the final code.

