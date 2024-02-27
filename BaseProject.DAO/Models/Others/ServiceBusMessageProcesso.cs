namespace BaseProject.DAO.Models.Others
{
    public class ServiceBusMessageProcesso<T>
    {
        public int IdProcesso { get; set; }
        public T Payload { get; set; }

        public ServiceBusMessageProcesso() { }

        public ServiceBusMessageProcesso(int idProcesso, T payload)
        {
            IdProcesso = idProcesso;
            Payload = payload;
        }
    }
}
