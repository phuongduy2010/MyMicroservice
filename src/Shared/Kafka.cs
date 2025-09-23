using Confluent.Kafka;
namespace Shared;

public static class Kafka
{
    public static IProducer<string, string> Producer(string bootstrap)
    {
        return new ProducerBuilder<string, string>(
             new ProducerConfig
             {
                 BootstrapServers = bootstrap,
                 Acks = Acks.All,
                 EnableIdempotence = true
             }).Build();
    }
    public static IConsumer<string, string> Consumer(string bootstrap, string groupId)
    {
        return new ConsumerBuilder<string, string>(
            new ConsumerConfig
            {
                BootstrapServers = bootstrap,
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            }).Build();
    }
}
       
