using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using DictionariesDAL.Interfaces;
using DictionariesDAL;
using DictionariesModel;
using Moq;
using Autofac;
using DictionariesBL.Confguration;
using System.Data.SqlClient;
using System.Reflection;
using System.Data;
using AlfaBank.Logger;

namespace DictionariesBL.Tests
{
    [TestClass()]
    public class BLServiceTests
    {
        #region fields
        private Mock<IRepository> _DataMock;
        private Mock<ILogger> _loggerMock;
        private IBLService service;
        private IContainer _autofacContainer;
        private Dictionary dict;
        private Item item;
        private List<Item> items=new List<Item>();
        private string dictionaryName = "currency";
        private string version = "1.0.0";
        private JObject value;
        #endregion fields
        #region autofac  
        protected IContainer AutofacContainer
        {
            get
            {
                if (_autofacContainer == null)
                {
                    var builder = new ContainerBuilder();
                    builder.RegisterModule<BLModule>();
                    builder.RegisterType<LogTruncator>().As<ILogTruncator>();
                    //builder.RegisterType<Log4NetLogger>().UsingConstructor(typeof(ILogTruncator));
                    builder.RegisterType<Log4NetLogger>().As<Mock<ILogger>>();
                    builder.RegisterType<DictionaryRepository>().As<Mock<IRepository>>();
                    builder.RegisterType<BLService>().As<IBLService>();
                    var container = builder.Build();
                    _autofacContainer = container;
                }
                return _autofacContainer;
            }
        }


        protected IBLService BLService
        {
            get { return AutofacContainer.Resolve<IBLService>(); }
        }
        protected IRepository DictionaryRepository
        {
            get { return AutofacContainer.Resolve<IRepository>(); }
        }
        protected ILogger Logger
        {
            get { return AutofacContainer.Resolve<ILogger>(); }
        }
        #endregion autofac
        #region init
        [TestInitialize]
        public void Init()
        {
            InitMocks();
            InitTestData();
            ConfigMock();
        }
        private void InitMocks()
        {
            _DataMock = new Mock<IRepository>();

            //_DataMock.Setup(t => t.Create(_currency));
            //_DataMock.Setup(t => t.Get()).Returns(_taskDataMock.Object);
            _loggerMock = new Mock<ILogger>();
            service = new BLService(_DataMock.Object, _loggerMock.Object);
        }
        private void ConfigMock()
        {
            _DataMock.Setup(t => t.GetItemAsync(dictionaryName, version, "1")).ReturnsAsync(item);
            _DataMock.Setup(t => t.GetDictionaryWithItemsAsync(dictionaryName, version)).ReturnsAsync(dict);
            _DataMock.Setup(t => t.GetDictionaryWithoutItemsAsync(dictionaryName, version)).ReturnsAsync(dict);
            _DataMock.Setup(t => t.GetDictionaryWithItems(dictionaryName, version)).Returns(dict);
            _DataMock.Setup(t => t.GetDictionaryWithoutItems(dictionaryName, version)).Returns(dict);
            //serviceMock.Setup(t => t.GetDictionaryAsync(dictionaryName, version,false)).ReturnsAsync(dict);
        }
        private void InitTestData()
        {
            int id = 1;
            value = JObject.Parse(@"{
                              'id':1,
                              'CurrencyCode': 'UAH',
                              'CurrencyName': 'гривня',
                              'CurrencySymbols' : '$'
                            }");
            item = new Item
            {
                ItemId = 1,
                DictionaryId = 1,
                Value = value
            };
            var listValue = new List<JObject>()
            {
                          JObject.Parse(@"{
                              'id':1,
                              'CurrencyCode': 'UAH',
                              'CurrencyName': 'гривня',
                              'CurrencySymbols' : '$'
                          }"),
                          JObject.Parse(@"{
                              'id':2,
                              'CurrencyCode': 'USD',
                              'CurrencyName': 'доллар',
                              'CurrencySymbols' : '$'
                          }"),
                          JObject.Parse(@"{
                              'id':3,
                              'CurrencyCode': 'RUB',
                              'CurrencyName': 'Рубль',
                              'CurrencySymbols' : '$'
                          }")
            };
            foreach (var v in listValue)
            {
                Item i = new Item()
                {
                    ItemId = id,
                    DictionaryId = 1,
                    Value = v
                };
                items.Add(i);
            }
            dict = new Dictionary()
            {
                Id=1,
                Name="currency",
                Version="1.0.0",
                Metadata="",
                Items=items
                
            };
        }
        #endregion



        [TestMethod()]
        public void GetDictionaryByNameAndVersionTest()
        {
            string name = "currency";
            string version = "1.0.0";
            var a = service.GetDictionaryAsync(name, version, true);
            string json = JsonConvert.SerializeObject(a, Formatting.Indented,
                                                        new JsonSerializerSettings
                                                        {
                                                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                                                        });
        }

        [TestMethod()]
        public void GetItemByIdTest()
        {
            string name = "currency";
            string version = "1.0.0";
            string id = "1";
            var a = service.GetItemAsync(name, version, id);

            string json = JsonConvert.SerializeObject(a, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }

        [TestMethod()]
        public void AddDictionaryTest()
        {
            string name = "currency";
            string version = "1.0.0";
            //string id = "1";
            DictionaryDTO dict = new DictionaryDTO
            {
                Name = name,
                Metadata = new JObject(),
                Version = version
            };
            service.CreateNewDictionaryAsync(dict);
            var a = service.GetDictionaryAsync(name, version, true);
            string json = JsonConvert.SerializeObject(a, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }

        [TestMethod()]
        public void AddItemTest()
        {
            string name = "currency";
            string version = "1.0.0";
            //string id = "1";         
            string value = @"{
                              
                              'c1': 'UAH',
                              'c2': 'гривня'
                          }";

            var json = JObject.Parse(value);
            var item = service.AddItemAsync(name, version, json);

        }




        [TestMethod()]
        public void AddItemAsyncWhenItemAlreadyExistsThrowExceptionTest()
        {

            _DataMock.Setup(x => x.AddAsync<Item>(It.IsAny<Item>())).Throws(CreateSqlException(2601));   
            Assert.ThrowsExceptionAsync<Exception>(async() => await service.AddItemAsync(dictionaryName, version, value));
            
        }
        [TestMethod()]
        public void AddItemAsyncWhenDictionaryNameOrVersionOrValueThrowExceptionTest()
        {
            _DataMock.Setup(x => x.AddAsync<Item>(It.IsAny<Item>())).ReturnsAsync(item);
            dictionaryName = null;
            version = null;
            value = null;
            Assert.ThrowsExceptionAsync<ArgumentException>(async () => await service.AddItemAsync(dictionaryName, version, value));
        }
        [TestMethod()]
        public void AddItemAsyncWhenDictionaryIsNullThrowExceptionTest()
        {
            _DataMock.Setup(t => t.GetDictionaryWithItemsAsync(dictionaryName, version)).ReturnsAsync((Dictionary)null);
            _DataMock.Setup(t => t.GetDictionaryWithoutItemsAsync(dictionaryName, version)).ReturnsAsync((Dictionary)null);



            _DataMock.Setup(x => x.AddAsync<Item>(It.IsAny<Item>())).ReturnsAsync(item);            
            Assert.ThrowsExceptionAsync<ArgumentException>(async () => await service.AddItemAsync(dictionaryName, version, value));
        }

        [TestMethod()]
        public void CreateNewDictionaryAsyncWhenValueIsNullThrowExceptionTest()
        {
            _DataMock.Setup(x => x.AddAsync<Item>(It.IsAny<Item>())).ReturnsAsync(item);
            value = null;
            Assert.ThrowsExceptionAsync<ArgumentException>(async () => await service.CreateNewDictionaryAsync(value.ToObject<DictionaryDTO>()));
        }

        public void CreateNewDictionaryAsyncWhenDictionaryAlreadyExistsThrowExceptionTest()
        {

            _DataMock.Setup(x => x.AddAsync<Dictionary>(It.IsAny<Dictionary>())).Throws(CreateSqlException(2601));
            Assert.ThrowsExceptionAsync<Exception>(async () => await service.CreateNewDictionaryAsync(value.ToObject<DictionaryDTO>()));

        }


        [TestMethod()]
        public void BLServiceTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ParseJsonTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AddItemTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ChangeDictionaryNameTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ChangeItemTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CreateNewDictionaryTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DeleteDictionaryTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DeleteItemTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DictionaryExistsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetDictionaryTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetItemTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetItemValueTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ItemExistsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetCountItemTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AddItemAsyncTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ChangeDictionaryNameAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ChangeItemAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CreateNewDictionaryAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DeleteDictionaryAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DeleteItemAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DictionaryExistsAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetDictionaryAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetItemAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetItemValueAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ItemExistsAsyncTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetCountItemAsyncTest()
        {
            Assert.Fail();
        }


        #region SqlException
        private SqlException CreateSqlException(int number)
        {
            var collectionConstructor = typeof(SqlErrorCollection)
              .GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, //visibility
              null, //binder
              new Type[0],
              null);
            var addMethod = typeof(SqlErrorCollection).GetMethod("Add", BindingFlags.NonPublic | BindingFlags.Instance);
            var errorCollection = (SqlErrorCollection)collectionConstructor.Invoke(null);
            var errorConstructor = typeof(SqlError).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null,
              new[]
              {
              typeof (int), typeof (byte), typeof (byte), typeof (string), typeof(string), typeof (string),
              typeof (int), typeof (uint)
              }, null);
            var error =
              errorConstructor.Invoke(new object[] { number, (byte)0, (byte)0, "server", "errMsg", "proccedure", 100, (uint)0 });
            addMethod.Invoke(errorCollection, new[] { error });
            var constructor = typeof(SqlException)
              .GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, //visibility
                null, //binder
                new[] { typeof(string), typeof(SqlErrorCollection), typeof(Exception), typeof(Guid) },
                null); //param modifiers
            return (SqlException)constructor.Invoke(new object[] { "Error message", errorCollection, new DataException(), Guid.NewGuid() });
        }
        #endregion SqlException
    }

}