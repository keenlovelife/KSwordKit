# 概述
这是一套完整的 Unity 开发套件，提供 UI 管理、热更新、资源管理等、多线程、数据处理、定时器、解压缩等功能完备的工具集，目的是为了提高生产力，方便业务开发，这套工具集被命名为 **KSwordKit** 。

未来的预期中也将加入更多游戏技术，比如帧同步等。

# 目前这个开发套件刚开坑，欢迎大家指正
刚开始构思这个开发套件，主要是总结一下游戏开发技术，提高工作效率，所以在这里开坑造轮子了，希望大家给出宝贵的意见。

# **KSwordKit** 层次图
![**KSwordKit**](https://github.com/keenlovelife/KSwordKit/blob/master/GitHub_Images/KSWordKit%20%E6%9E%B6%E6%9E%84%E8%AE%BE%E8%AE%A1.jpg?raw=true)

图中可以看出 **KSwordKit** 位于 Unity 之上，具体应用程序之下。 **KSwordKit** 试图为你的应用程序提供功能完备，接口丰富的快速开发工具包。

为实现这一目的， **KSwordKit** 内部的所有内容将被设计成 `独立赖可插拔` 的组件化系统，能够让开发者根据需要增删内容来精简自己的项目。

图中也可以看出 **KSwordKit** 内部分为两部分：`Basic` 和 `Framework`

1. `Basic` 内部提供大量丰富的工具类库，每个工具类库解决它对应的具体程序需求。 
2. `Framework` 是构建在 `Basic` 之上的功能组件，每个组件针对具体业务场景提供完成解决方案。

而不管是 `Basic` 还是 `Framework` ，它们内部的所有内容，都被 **KSwordKit** 看作 `独立可插拔` 的组件，为了区分层次，把所有组件分为两类：`Basic` 内部组件的功能是提供基础功能，比如：算法、数据处理、异步能力等; `Framework` 内部的组件则建立在 `Basic` 之上，为具体业务需求而设计的解决方案，如热更新框架、资源管理框架、UI框架等。

# 组件遵循 `独立可插拔` 原则

一个独立组件是可插拔的，即可以物理上删除或添加，它可以依赖其他模块，拥有自己的界面和代码。而组件间的引用依赖问题，则由 **KSwordKit** 自己管理。

**KSwordKit** 将会提供很多种组件，它们的设计模式、功能和依赖都会不尽相同。但这些组件都需要遵循同一个原则：**独立可插拔**

每个组件应当是独立的，以便可以独立使用或者复制到别的项目就可以直接使用的，这就是所谓 `可插拔`。这样可以方便开发者根据需求自定义增减内容，框架更包容，不排他。

>组件：有的人叫做工具，有的人则叫做模块，有的被称为插件，在这里它们都被称为组件。

如有朋友设计并实现了某组件，欢迎给仓库提交PR或者提交Issues，大家共同交流技术。

# 文档
**KSwordKit** 里面的每部分都会在 [Wiki](https://github.com/keenlovelife/KSwordKit/wiki) 里面撰写文档，大家使用中遇到问题可以去查阅。

# 当前已撰写的文档
1. [如何在项目中使用KSwordKit？](https://github.com/keenlovelife/KSwordKit/wiki/%E5%A6%82%E4%BD%95%E5%9C%A8%E9%A1%B9%E7%9B%AE%E4%B8%AD%E4%BD%BF%E7%94%A8KSwordKit%EF%BC%9F)
2. [组件管理](https://github.com/keenlovelife/KSwordKit/wiki/%E7%BB%84%E4%BB%B6%E7%AE%A1%E7%90%86)
3. [增强协程](https://github.com/keenlovelife/KSwordKit/wiki/%E5%A2%9E%E5%BC%BA%E5%8D%8F%E7%A8%8B)
4. [资源管理器](https://github.com/keenlovelife/KSwordKit/wiki/%E8%B5%84%E6%BA%90%E7%AE%A1%E7%90%86%E5%99%A8)