# ZmSkill
贼猫unity游戏技能框架

该教程已过时，请下载工程中的教程(如果有的话)

该框架允许你简单的创建属于自己的个性化技能
开始：
在ZmSkill\userdata文件夹下的所有内容均为开发者自定义的技能类，工程中带有一些示范内容。

首先为需要使用技能的物体对象附加以下脚本:
1.SkillSystem
该脚本负责技能系统的调度，是该技能框架的核心

2.Role
技能大多数情况下会涉及到角色属性，该对象为角色属性，同时也负责用于读取已学会的技能列表，开发者可以根据自己的需要进行自定义
如果你已经拥有了自己的属性对象，那么你可以继承Role

定义一个技能，一般来说只需要创建一个C#类即可
1.创建一个C#类

2.删除Start与Update方法(如果存在)

3.继承SkillBase类

4.实现所有的抽象类

至此一个技能理论上便已经创建完毕，接下来我将解释所有需要实现的方法的含义

Effect()
效果，该方法在技能习得的情况下，该方法会被循环调用，用于实现被动技能

GetId()
返回技能的id，该框架没有自动编排技能id的功能，需要手动写下一个不重复的id用于区分技能

GetName()
技能的名字

GetOp(OpCode newOpCode )
参数:newOpCode 用于自定义接收，未来会取消
该框架可以使用动作类型来触发技能，该方法用于定义可接受的动作类型，该方法考虑到其实是意义不大的，未来版本中会删除，建议不使用,此处返回空OpCode集合即可

GetSkillType()
返回技能的类型，目前定义有主动技能和被动技能两项，开发者可以更具需要自行定义，示范中的active与passive当前并无实际作用

OpEffect_Factory(OpCode opCode, Role otherRole, params float[] values)
该方法用于调用技能，过时方法，未来版本中删除
参数:
opCode 发动技能时使用的
otherRole 受到技能效果影响的其他对象
values 技能发动时附加的参数

Use_Factory()
主动调用技能效果
未来该方法取代OpEffect_Factory()
