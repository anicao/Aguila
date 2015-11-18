using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace CentralAgentesWCF
{
    [ServiceContract]
    public interface ICotizacionService
    {
        [OperationContract]
        bool SendCotizacionByMail(string cotizacionID);
    }
}
