using System;
using DictionariesBL;
using DictionariesModel;
using DictionaryApi.Filters;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using AlfaBank.Logger;

namespace DictionaryApi.Controllers
{
    /// <summary>
    /// Dictionary Controller
    /// </summary>
    //[ApiAuthenticationFilter]
    [RoutePrefix("dictionary")]
    [ExceptionFilter]
    public class DictionaryController : ApiController
    {
        private readonly ILogger _logger;
        private readonly IBLService _service;

        /// <summary>
        /// Dictionary Controller
        /// </summary>
        /// <param name="service"></param>
        /// <param name="logger"></param>
        public DictionaryController(IBLService service, ILogger logger)
        {
            _service = service;

            _logger = logger;
        }

        /// <summary>
        /// Get Dictionaries by dictionaryName
        /// </summary>
        /// <param name="dictionaryName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("v1/{dictionaryName}/")]
        [Route("v2/{dictionaryName}/")]
        [ResponseType(typeof(List<DictionaryDTO>))]
        public async Task<HttpResponseMessage> GetDictionaryByName(string dictionaryName)
        {
            _logger.Info(
                $"DictionaryController.GetDictionaryByName [ClientAddress: {GetClientAddress()}, DictionaryName: {dictionaryName}]");

            try
            {
                var res = await _service.GetDictionariesAsync(dictionaryName);

                return Request.CreateResponse(res == null ? HttpStatusCode.NoContent : HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                _logger.Error(GetErrorMessage(ex));

                return Request.CreateResponse(HttpStatusCode.InternalServerError, GetErrorMessage(ex));
            }

        }

        // GET api/<controller>
        /// <summary>
        /// Get Dictionary
        /// </summary>
        /// <param name="dictionaryName"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("v1/{dictionaryName}/{version=1.0.0}")]
        [ResponseType(typeof(DictionaryDTO))]
        public async Task<HttpResponseMessage> GetDictionaryByNameAndVersion(string dictionaryName, string version)
        {
            _logger.Info(
                $"DictionaryController.GetDictionaryByNameAndVersion [ClientAddress: {GetClientAddress()}, DictionaryName: {dictionaryName}, Version: {version}]");
            try
            {
                var res = await _service.GetDictionaryAsync(dictionaryName, version, true);

                return Request.CreateResponse(res == null ? HttpStatusCode.NoContent : HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                _logger.Error(GetErrorMessage(ex));

                return Request.CreateResponse(HttpStatusCode.InternalServerError, GetErrorMessage(ex));
            }
        }

        // GET api/<controller>/5
        /// <summary>
        /// Get Item
        /// </summary>
        /// <param name="dictionaryName"></param>
        /// <param name="version"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("v1/{dictionaryName}/{version=1.0.0}/{id}")]
        [ResponseType(typeof(Item))]
        public async Task<HttpResponseMessage> GetDictionaryItemById(string dictionaryName, string version, string id)
        {
            _logger.Info(
                $"DictionaryController.GetDictionaryItemById [ClientAddress: {GetClientAddress()}, DictionaryName: {dictionaryName}, Version: {version}, Id: {id}]");
            try
            {
                var res = await _service.GetItemAsync(dictionaryName, version, id);

                if (res == null)
                {
                    Request.CreateResponse(HttpStatusCode.BadRequest, (Item)null);
                }

                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                _logger.Error(GetErrorMessage(ex));

                return Request.CreateResponse(HttpStatusCode.InternalServerError, GetErrorMessage(ex));
            }
        }

        // POST api/<controller>
        /// <summary>
        /// Create Dictionary
        /// </summary>
        /// <param name="dictionaryDto"></param>
        [HttpPost]
        [Route("v1/")]
        [ResponseType(typeof(DictionaryDTO))]
        public async Task<HttpResponseMessage> CreateDictionary(DictionaryDTO dictionaryDto)
        {
            if (dictionaryDto == null)
            {
                _logger.Info(
                    $"DictionaryController.CreateDictionary [ClientAddress: {GetClientAddress()}]");

                throw new ApplicationException("Object DictionaryDTO is null.");
            }

            _logger.Info(
                $"DictionaryController.CreateDictionary [ClientAddress: {GetClientAddress()}, " +
                $"DictionaryName: {dictionaryDto.Name}, " +
                $"Version: {dictionaryDto.Version}, " +
                $"Id: {dictionaryDto.Id}]");

            try
            {
                var res = await _service.CreateNewDictionaryAsync(dictionaryDto);

                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                _logger.Error(GetErrorMessage(ex));

                return Request.CreateResponse(HttpStatusCode.InternalServerError, GetErrorMessage(ex));
            }
        }

        /// <summary>
        /// Add Item
        /// </summary>
        /// <param name="dictionaryName"></param>
        /// <param name="version"></param>
        /// <param name="value"></param>        
        [HttpPost]
        [Route("v1/{dictionaryName}/{version=1.0.0}/items")]
        [ResponseType(typeof(Item))]
        public async Task<HttpResponseMessage> PostItem(string dictionaryName, string version, JObject value)
        {
            _logger.Info(
               $"DictionaryController.PostItem [ClientAddress: {GetClientAddress()}, DictionaryName: {dictionaryName}, Version: {version}]");

            try
            {
                var res = await _service.AddItemAsync(dictionaryName, version, value);

                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                _logger.Error(GetErrorMessage(ex));

                return Request.CreateResponse(HttpStatusCode.InternalServerError, GetErrorMessage(ex));
            }
        }

        /// <summary>
        /// change item
        /// </summary>
        /// <param name="dictionaryName"></param>
        /// <param name="version"></param>
        /// <param name="id"></param>
        /// <param name="item"></param>
        [HttpPut]
        [Route("v1/{dictionaryName}/{version=1.0.0}/items/{id}")]
        [ResponseType(typeof(Item))]
        public async Task<HttpResponseMessage> ChangeItem(string dictionaryName, string version, string id, JObject item)
        {
            _logger.Info(
              $"DictionaryController.ChangeItem [ClientAddress: {GetClientAddress()}, DictionaryName: {dictionaryName}, Version: {version}, Id: {id}]");

            try
            {
                var res = await _service.ChangeItemAsync(dictionaryName, version, id, item);

                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                _logger.Error(GetErrorMessage(ex));

                return Request.CreateResponse(HttpStatusCode.InternalServerError, GetErrorMessage(ex));
            }
        }

        /// <summary>
        /// Puts the dictionary. Change or create new dictionary.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary.</param>
        /// <param name="version">The version.</param>
        /// <param name="dictionary">The dictionary.</param>
        /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
        [HttpPut]
        [Route("v1/{dictionaryName}/{version=1.0.0}/")]
        [ResponseType(typeof(DictionaryDTO))]
        public async Task<HttpResponseMessage> PutDictionary(
            string dictionaryName, string version, DictionaryDTO dictionary)
        {
            _logger.Info(
                "DictionaryController.PutDictionary [" +
                $"ClientAddress: {GetClientAddress()}, DictionaryName: {dictionaryName}, Version: {version}]");

            try
            {
                var res = await _service.ChangeDictionaryAsync(dictionaryName, version, dictionary);

                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                _logger.Error(GetErrorMessage(ex));

                return Request.CreateResponse(HttpStatusCode.InternalServerError, GetErrorMessage(ex));
            }
        }

        /// <summary>
        /// Puts the dictionary. Change or create new dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <returns>
        /// Task&lt;HttpResponseMessage&gt;.
        /// </returns>
        [HttpPut]
        [Route("v1/")]
        [ResponseType(typeof(DictionaryDTO))]
        public async Task<HttpResponseMessage> PutDictionary(DictionaryDTO dictionary)
        {
            if (dictionary == null)
            {
                _logger.Info(
                    $"DictionaryController.PutDictionary [ClientAddress: {GetClientAddress()}]");

                throw new ApplicationException("Object DictionaryDTO is null.");
            }

            _logger.Info(
                "DictionaryController.PutDictionary [" +
                $"ClientAddress: {GetClientAddress()}, DictionaryName: {dictionary.Name}, Version: {dictionary.Version}]");

            try
            {
                var res = await _service.ChangeDictionaryAsync(dictionary.Name, dictionary.Version, dictionary);

                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                _logger.Error(GetErrorMessage(ex));

                return Request.CreateResponse(HttpStatusCode.InternalServerError, GetErrorMessage(ex));
            }
        }
        
        /// <summary>
        /// Delete Dictionary
        /// </summary>
        /// <param name="dictionaryName"></param>
        /// <param name="version"></param>
        [HttpDelete]
        [Route("v1/{dictionaryName}/{version=1.0.0}/")]
        [ResponseType(typeof(bool))]
        public async Task<HttpResponseMessage> DeleteDictionary(string dictionaryName, string version)
        {
            _logger.Info(
                $"DictionaryController.DeleteDictionary [ClientAddress: {GetClientAddress()}, DictionaryName: {dictionaryName}, Version: {version}]");

            try
            {
                var res = await _service.DeleteDictionaryAsync(dictionaryName, version);

                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                _logger.Error(GetErrorMessage(ex));

                return Request.CreateResponse(HttpStatusCode.InternalServerError, GetErrorMessage(ex));
            }
        }

        /// <summary>
        /// Delete Item
        /// </summary>
        /// <param name="dictionaryName"></param>
        /// <param name="version"></param>
        /// <param name="id"></param>
        [Route("v1/{dictionaryName}/{version=1.0.0}/items/{id}")]
        [ResponseType(typeof(bool))]
        public async Task<HttpResponseMessage> DeleteDictionaryItem(string dictionaryName, string version, string id)
        {
            _logger.Info(
                $"DictionaryController.DeleteDictionaryItem [ClientAddress: {GetClientAddress()}, DictionaryName: {dictionaryName}, Version: {version}, Id: {id}]");

            try
            {
                var res = await _service.DeleteItemAsync(dictionaryName, version, id);

                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                _logger.Error(GetErrorMessage(ex));

                return Request.CreateResponse(HttpStatusCode.InternalServerError, GetErrorMessage(ex));
            }
        }

        /// <summary>
        /// Gets all dictionaries without items
        /// </summary>
        /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
        [Route("v1/")]
        [HttpGet]
        [ResponseType(typeof(List<DictionaryDTO>))]
        public async Task<HttpResponseMessage> GetAllDictionaries()
        {
            _logger.Info(
                $"DictionaryController.GetAllDictionaries [ClientAddress: {GetClientAddress()}]");
            try
            {
                var res = await _service.GetAllDictionaries();

                return Request.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception ex)
            {
                _logger.Error(GetErrorMessage(ex));

                return Request.CreateResponse(HttpStatusCode.InternalServerError, GetErrorMessage(ex));
            }
        }

        #region private members

        private string GetClientAddress()
        {
            if (Request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper)Request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }
            if (!Request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            {
                return HttpContext.Current != null
                    ? HttpContext.Current.Request.UserHostAddress
                    : null;
            }

            var prop = (RemoteEndpointMessageProperty)Request.Properties[RemoteEndpointMessageProperty.Name];

            return prop.Address;
        }

        private static string GetInnerException(Exception exception)
        {
            var message = string.Empty;

            if (exception?.InnerException != null)
            {
                message += $" {exception.InnerException.Message} {GetInnerException(exception.InnerException)}";
            }

            return message;
        }

        private static string GetErrorMessage(Exception ex)
        {
            return $"{ex.Message}. {GetInnerException(ex.InnerException)}";
        }
        #endregion
    }
}