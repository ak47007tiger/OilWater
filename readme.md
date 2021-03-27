## Unity可交互水波纹后处理
- 基于unity后处理，提供可交互的屏幕水波纹效果
- git地址：https://github.com/ak47007tiger/OilWater
- 效果图
![在这里插入图片描述](https://img-blog.csdnimg.cn/20210327190914828.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L2FrNDcwMDd0aWdlcg==,size_16,color_FFFFFF,t_70#pic_center)


## 特性
- 提供2个算法：一个来自shader toy；一个网上找的经典算法
  - 我是实用主义，如果有现成的算法，我不会从0去推算，看懂之后我只关心其使用价值
- 自动适配超大屏幕，不会屏幕越大越卡
- 关键参数都提供配置，不用写代码

## 下载了工程怎么看效果
- 打开Assets/OilWave/Loading.unity，运行
- 快捷键"control + i"进行场景切换，在有图的场景点击屏幕即可

## 原理
- 说这个原理是在一种后视镜的角度说的，即事后诸葛亮，这里主要帮助读者理解，我无法复现创造者的思路
- 水波纹是一种震动的传播，我们需要创建震动、扩散震动
- 水面产生波纹后有高低起伏，镜面反射、折射，若要逼真，我们需要实现这些效果，本文不管
- 自然现象中水纹圈上可以看到水面下其它地方的景象，以下称扭曲，这个现象这是本文要实现的

## 实现思路
- 假设一张纹理是平静的水面
- 产生初始震动，设置一个纹理的某个区域的r通道为某个值
- 传播震动，每帧更新这个纹理，让这个区域的r通道的值能像一个圆环一样扩散出去，需要知道上一帧的值，不能当前帧更新后就丢弃上一帧的值，所以还需要g通道保存上一帧的震动值
- 波动越大，扭曲越厉害，即震动纹理中代表震动幅度的像素值越大，对要显示的图片采样的时候偏移越大

## 结合Unity的实现思路
- 震动纹理：创建一个纹理，每帧调用shader读取并且渲染这个纹理
  - 发现CustomRenderTexture封装了这个功能，直接用，指定shaderPass为传播震动的pass，具体可以看工程代码
- 产生振幅
  - CustomRenderTexutre在重置UpdateZones的时候会执行所有的pass，增加一个把点击转换到纹理坐标的功能，并且把坐标传到材质里面，执行点击pass的时候就会设置一个初始的震动到震动纹理的某个像素
- 让整个场景可以产生交互水波纹，对游戏场景产生的纹理进行采样扭曲，后处理可以满足这个需求
- 扭曲公式
  - 输入
    - x,y
    - 振幅纹理waterTex
    - 场景纹理mainTex
  - 中间值
    - 目标像素上下左右的振幅, l, r, t, b
    - offset = normalize(float3(r - l, t - b, 1))
  - 有了offset就可以对mainTex进行有扭曲的采样

## 需要的unity知识
- 材质属性设置
- shader语法
- CustomRenderTexutre特性
- 自定义后处理流程

## 工程中主要的类说明
- 代码都在Assets/OilWave下，其它文件夹下是无关代码，没意义，不用管
  - ConfigManager，为了程序打包后在不改代码的前提下修改材质参数而写
  - DemoForResolutionAdapt，负责创建CustomRenderTexture，如果屏幕过大会缩小创建的震动纹理来提高性能
  - EffectInput，适配pc和移动端，处理鼠标点击或者手指触摸事件
  - OilWavePostProcessing，更新震动纹理，执行后处理
  - ProcessingConfig，参数类，用于创建数据对象
  - WaterManager，管理资源，适配屏幕大小切换的情况
  - WaterWave01，最早的demo版本，没有这么多业务逻辑
- WaterWave01.shader，更新震动纹理的shader
- WaterWaveShow.shader，后处理显示shader，扭曲采样的逻辑在这里
