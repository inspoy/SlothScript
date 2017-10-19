# SlothScript

一个简单的脚本语言，练习编译原理用

基本上就是图书《两周自制脚本语言》的读书笔记？

其实就是跟着书自己实践一遍

## 目录说明
* SlothScript - 脚本解释器
* UnitRunner - 单元测试

## 关于脚本

### 基本语法

简单样例
```
sum = 0;
i = 1;
while i <= 10 do
  if i % 2 == 0 do
    sum = sum + i; // 计算2+4+...+10
  end;
  i = i + 1; // 循环计数器
end;
return sum; // 返回结果
```
1. 双斜杠//表示注释
2. 每个语句后都用分号隔开
3. 支持的运算符包括+ - * / % > < >= <= ==
4. 数据类型暂时只支持整数和字符串，字符串用双引号括起
5. if和while语句为<关键字><表达式>do<语句块>end;
6. return语句中断运行并返回表达式的值

函数支持
```
// 定义
def fib(n) do
  if (n == 0)||(n == 1) do
    return 1;
  end;
  return fib(n-2) + fib(n-1);
end;
// 调用，计算斐波那契数列
return fib(10);
```

### 如何使用
详见UnitRunner的各种Usage