# 概述
这是一套完整的 Unity 开发套件，计划提供 UI 管理、热更新、资源管理等、多线程、数据处理、定时器、解压缩等功能完备的工具集，目的是为了提高生产力，方便业务开发。

未来的预期中也将加入更多游戏技术，比如帧同步，GPU实例渲染等。

# 目前这个开发套件刚开坑，欢迎大家指正
刚开始构思这个开发套件，主要是积累沉淀一下游戏开发技术，提高工作效率，所以在这里开坑造轮子了，大家如果有任何想法或者建议都可以与我交流。

# **KSwordKit** 层次图
![**KSwordKit**](https://github.com/keenlovelife/KSwordKit/blob/main/GitHub_Images/KSWordKit%20%E6%9E%B6%E6%9E%84%E8%AE%BE%E8%AE%A1.jpg?raw=true)

图中可以看出 **KSwordKit** 位于 Unity 之上，具体应用程序之下。 **KSwordKit** 试图为你的应用程序提供功能完备，接口丰富的快速开发工具包。

为实现这一目的， **KSwordKit** 内部的所有内容将被设计成 `独立可插拔` 的包系统，能够让开发者根据需求增删内容来精简自己的项目。

图中也可以看出 **KSwordKit** 内部分为两部分：`Basic` 和 `Framework`

1. `Basic` 提供大量丰富的工具类库，每个工具类库解决它对应的具体程序需求。 
2. `Framework` 是构建在 `Basic` 之上的功能包，针对具体业务场景提供完整解决方案。

而不管是 `Basic` 还是 `Framework` ，它们内部的所有内容，都被 **KSwordKit** 看作 `独立可插拔` 的包，为了便于区分，在概念上把所有包分为两类：`Basic` 内部组件的功能是提供基础功能，比如：算法、数据处理、异步能力等; `Framework` 内部的组件则建立在 `Basic` 之上，为具体业务需求而设计的解决方案，如热更新框架、资源管理框架、UI框架等。


#  `独立可插拔` 原则

一个独立的包是可插拔的，即可以物理上删除或添加，它可以依赖其他包，也可以拥有自己的界面和代码。而包与包之间间的引用依赖问题，则由 **KSwordKit** 自己管理。

**KSwordKit** 将会提供很多种不同的包，它们的设计模式、功能和依赖都会不尽相同。但它们都需要遵循同一个原则：**独立可插拔**

这里的核心想法是：每个包应当是独立的，以便可以独立使用，或者复制到别的项目就可以直接使用的，这就是所谓 `可插拔`。这样可以方便开发者根据需求自定义增减内容，框架更包容，不排他，也更能容易满足实际开发过程中多变的需求。

如有朋友设计并实现了某组件，欢迎给仓库提交PR或者提交Issues，大家共同交流技术。

# 文档
**KSwordKit** 里面的每部分都会在 [Wiki](https://github.com/keenlovelife/KSwordKit/wiki) 里面撰写文档，大家使用中遇到问题可以去查阅。

# 当前已撰写的文档
1. [如何在项目中使用KSwordKit？](https://github.com/keenlovelife/KSwordKit/wiki/%E5%A6%82%E4%BD%95%E5%9C%A8%E9%A1%B9%E7%9B%AE%E4%B8%AD%E4%BD%BF%E7%94%A8KSwordKit%EF%BC%9F)
2. [包管理器](https://github.com/keenlovelife/KSwordKit/wiki/%E7%BB%84%E4%BB%B6%E7%AE%A1%E7%90%86)