namespace CompanyApi.Model
{
    public class ConvertBody<T>
    {
        public T subject { get; set; }

        public ConvertBody(T subject)
        {
            this.subject = subject;
        }
    }
}
