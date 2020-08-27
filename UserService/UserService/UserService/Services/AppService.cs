using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.Services
{
    public class AppService
    {
        public static string SecretGenerate(Int64 id)
        {
            return MD5Service.MD5Hash(id + "FaceChatMD5EncrypterSecret");
        }
    }
}
