using System.Collections.Generic;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public class BaseResponse
    {
        public Dictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();

    }
    public class BaseResponse<T> : BaseResponse
    {
        public BaseResponse()
        {

        }
        public BaseResponse(T data,int total = 0,string nextUrl = "")
        {
            Data = data;
            if (data is System.Collections.ICollection enumVar)
            {
                Count = enumVar.Count;
            }
            else
            {
                Count = Data != null ? 1 : 0;
            }
            Total = total;
            Next = nextUrl;
        }
        public T Data { get; set; }
        public int Count { get; private set; }
        public int Total { get; private set; }
        public string Next { get; private set; }
    }
}
