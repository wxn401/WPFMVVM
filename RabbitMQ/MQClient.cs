using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Windows.Controls;

namespace WpfCore.RabbitMQ
{
    public class MQClient
    {
        public MQClient()
        {
            factory = new ConnectionFactory();
            factory.HostName = "localhost";//RabbitMQ服务在本地运行
            factory.UserName = "guest";//用户名
            factory.Password = "guest";//密码
        }

        public ConnectionFactory factory { get; set; }
        public string queueName = "test";
        public string exchangeName = "ttl.exc";
        public string dqueueName = "dtest";
        public string dexchangeName = "ttl.dexc";
        public int count = 10;
        public bool Send()
        {
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    #region 生产者
                    //创建死信交换机
                    channel.ExchangeDeclare(dexchangeName, type: ExchangeType.Direct, durable: true, autoDelete: false);
                    var dArgs = new Dictionary<string, object>
                    {
                        { "x-message-ttl", 60000 } // 消息超时时间设置为60秒
                    };
                    //创建死信队列
                    channel.QueueDeclare(dqueueName, durable: true, exclusive: false, autoDelete: false, dArgs);
                    //死信队列绑定死信交换机
                    channel.QueueBind(dqueueName, dexchangeName, routingKey: dqueueName);

                    // 定义队列
                    var normalArgs = new Dictionary<string, object>
                    {
                        { "x-dead-letter-exchange", dexchangeName },
                        { "x-dead-letter-routing-key", dqueueName },
                        { "x-message-ttl", 6000 } // 消息超时时间设置为60秒
                    };
                    // 定义队列 ,消息分发规则由 ExchangeType 确定
                    //direct（明确的路由规则：消费端绑定的队列名称必须和消息发布时指定的路由名称一致）
                    //topic （模式匹配的路由规则：支持通配符）
                    //fanout （消息广播，将消息分发到exchange上绑定的所有队列上）
                    channel.ExchangeDeclare(exchangeName, type: ExchangeType.Direct, durable: true, autoDelete: false);
                    //创建死信队列
                    channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, normalArgs);
                    //设置prefetchCount : 1来告知RabbitMQ，在未收到消费端的消息确认时，不再分发消息，也就确保了当消费端处于忙碌状态时
                    channel.BasicQos(0, 1, false);
                    //死信队列绑定死信交换机
                    channel.QueueBind(queueName, exchangeName, routingKey: queueName);

                    // 发送消息
                    
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;
                    //properties.Expiration = "5000"; // 消息过期时间设置为5秒
                    //开启消息确认模式
                    channel.ConfirmSelect();

                    channel.BasicAcks += new EventHandler<BasicAckEventArgs>((o, b) =>
                    {
                        //deliveryTag；唯一消息标签
                        //multiple：是否批量
                        Console.WriteLine($"调用ack回调方法: DeliveryTag: {b.DeliveryTag};Multiple: {b.Multiple}");
                    });

                    channel.BasicNacks += new EventHandler<BasicNackEventArgs>((o, b) =>
                    {
                        //deliveryTag；唯一消息标签
                        //multiple：是否批量
                        Console.WriteLine($"调用Nacks回调方法; DeliveryTag: {b.DeliveryTag};Multiple: {b.Multiple}");
                    });

                    for (int i = 0;i < count; i++)
                    {
                        var message = "Hello RabbitMQ!-" + i;
                        var body = Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish(exchangeName, queueName, properties, body);
                    }

                    //channel.WaitForConfirms();
                    //channel.WaitForConfirmsOrDie();//如果所有消息发送成功 就正常执行；如果有消息发送失败；就抛出异常；
                    #endregion
                    //int c = 1000;
                    //while (c > 0)
                    //{
                    //    c--;
                    //    Thread.Sleep(2000);
                    //}
                    Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")}\t运行结束。");
                    //Console.ReadKey();

                }
            }

            return true;
        }

        public bool Comsumer()
        {
            using (var con = factory.CreateConnection())
            {
                using (var channel = con.CreateModel())
                {
                    #region 消费者 
                    // 定义死信交换机
                    channel.ExchangeDeclare(dexchangeName, type: ExchangeType.Direct, durable: true, autoDelete: false);
                    var dArgs = new Dictionary<string, object>
                    {
                        { "x-message-ttl", 60000 } // 消息超时时间设置为60秒
                    };
                    //创建死信队列
                    channel.QueueDeclare(dqueueName, durable: true, exclusive: false, autoDelete: false, dArgs);
                    //死信队列绑定死信交换机
                    channel.QueueBind(dqueueName, dexchangeName, routingKey: dqueueName);

                    // 定义队列
                    var normalArgs = new Dictionary<string, object>
                    {
                        { "x-dead-letter-exchange", dexchangeName },
                        { "x-dead-letter-routing-key", dqueueName },
                        { "x-message-ttl", 6000 } // 消息超时时间设置为60秒
                    };
                    //autoDelete 自动移除消息
                    channel.ExchangeDeclare(exchangeName, type: ExchangeType.Direct, durable: true, autoDelete: false);
                    //创建死信队列
                    channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, normalArgs);
                    //死信队列绑定死信交换机
                    channel.QueueBind(queueName, exchangeName, routingKey: queueName);

                    //设置prefetchCount : 1来告知RabbitMQ，在未收到消费端的消息确认时，不再分发消息，也就确保了当消费端处于忙碌状态时
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: true);
                    // 定义消息消费者
                    var dconsumer = new EventingBasicConsumer(channel);
                    dconsumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body.ToArray());
                        Console.WriteLine($"接收到消息：{message}");
                        //不ack(BasicNack),且不把消息放回队列(requeue:false)
                        channel.BasicNack(ea.DeliveryTag, false, requeue: false);
                    };
                    //autoAck:true；自动进行消息确认，当消费端接收到消息后，就自动发送ack信号，不管消息是否正确处理完毕
                    //autoAck:false；关闭自动消息确认，通过调用BasicAck方法手动进行消息确认
                    channel.BasicConsume(dqueueName, false, dconsumer);

                    int c = 1000;
                    while(c > 0)
                    {
                        c--;
                        Thread.Sleep(2000);
                    }

                    #endregion
                }
            }

            return true;
        }

    }
}
