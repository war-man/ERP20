﻿using AutoMapper;
using ERP.API.Models;
using ERP.Common.Constants;
using ERP.Common.Excel;
using ERP.Common.Models;
using ERP.Data.Dto;
using ERP.Data.ModelsERP;
using ERP.Data.ModelsERP.ModelView;
using ERP.Data.ModelsERP.ModelView.ExportDB;
using ERP.Data.ModelsERP.ModelView.OrderService;
using ERP.Data.ModelsERP.ModelView.Service;
using ERP.Extension.Extensions;
using ERP.Service.Services.IServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ERP.API.Controllers.Dashboard
{
    [EnableCors("*", "*", "*")]
    [Authorize]
    public class ManagerCustomerOrderController : BaseController
    {
        private readonly ICustomerOrderService _customer_orderservice;
        private readonly ICustomerService _customerservice;
        private readonly IOrderProductService _order_productservice;
        private readonly IOrderServiceService _orderserviceservice;
        private readonly IShipAddressService _shipaddressservice;
        private readonly IExecutorService _executorservice;
        private readonly IServiceTimeService _servicetimeservice;

        private readonly IMapper _mapper;

        public ManagerCustomerOrderController()
        {

        }
        public ManagerCustomerOrderController(IServiceTimeService servicetimeservice,IExecutorService executorservice,IOrderServiceService orderserviceservice,ICustomerOrderService customer_orderservice, ICustomerService customerservice, IOrderProductService order_productservice, IShipAddressService shipAddressService, IMapper mapper)
        {

            this._order_productservice = order_productservice;
            this._shipaddressservice = shipAddressService;
            this._customer_orderservice = customer_orderservice;
            this._customerservice = customerservice;
            this._orderserviceservice = orderserviceservice;
            this._executorservice = executorservice;
            this._servicetimeservice = servicetimeservice;
            this._mapper = mapper;

        }


        #region methods
        [HttpGet]
        [Route("api/customer-orders/infor")]
        public IHttpActionResult GetInforById(int id)
        {
            ResponseDataDTO<customerordermodelview> response = new ResponseDataDTO<customerordermodelview>();
            try
            {
                response.Code = HttpCode.OK;
                response.Message = MessageResponse.SUCCESS;
                response.Data = _customer_orderservice.GetAllOrderById(id);
            }
            catch (Exception ex)
            {
                response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                response.Message = ex.Message;
                response.Data = null;

                Console.WriteLine(ex.ToString());
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("api/customer-orders/status")]
        public IHttpActionResult GetllStatus()
        {
            ResponseDataDTO<List<dropdown>> response = new ResponseDataDTO<List<dropdown>>();
            try
            {
                response.Code = HttpCode.OK;
                response.Message = MessageResponse.SUCCESS;
                response.Data = _customer_orderservice.GetAllStatus();
            }
            catch (Exception ex)
            {
                response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                response.Message = ex.Message;
                response.Data = null;

                Console.WriteLine(ex.ToString());
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("api/customer-orders/get-all-payment")]
        public IHttpActionResult GetAllSPayment()
        {
            ResponseDataDTO<List<dropdown>> response = new ResponseDataDTO<List<dropdown>>();
            try
            {
                response.Code = HttpCode.OK;
                response.Message = MessageResponse.SUCCESS;
                response.Data = _customer_orderservice.GetAllPayment();
            }
            catch (Exception ex)
            {
                response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                response.Message = ex.Message;
                response.Data = null;

                Console.WriteLine(ex.ToString());
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("api/customer-orders/search")]
        public IHttpActionResult GetAllSearch(int pageNumber, int pageSize, int? payment_type_id, DateTime? start_date, DateTime? end_date, string code)
        {
            ResponseDataDTO<PagedResults<customerorderviewmodel>> response = new ResponseDataDTO<PagedResults<customerorderviewmodel>>();
            try
            {
                response.Code = HttpCode.OK;
                response.Message = MessageResponse.SUCCESS;
                response.Data = _customer_orderservice.GetAllSearch(pageNumber: pageNumber, pageSize: pageSize, payment_type_id: payment_type_id,start_date,end_date, code);
            }
            catch (Exception ex)
            {
                response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                response.Message = "Không tìm thấy";
                response.Data = null;

                Console.WriteLine(ex.ToString());
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("api/customer-order-service/search")]
        public IHttpActionResult GetAllSearchCustomerOrderService(int pageNumber, int pageSize, DateTime? start_date, DateTime? end_date, string search_name)
        {
            ResponseDataDTO<PagedResults<servicercustomerorderviewmodel>> response = new ResponseDataDTO<PagedResults<servicercustomerorderviewmodel>>();
            try
            {
                response.Code = HttpCode.OK;
                response.Message = MessageResponse.SUCCESS;
                response.Data = _customer_orderservice.GetAllSearchCustomerOrderService(pageNumber: pageNumber, pageSize: pageSize, start_date,end_date, search_name);
            }
            catch (Exception ex)
            {
                response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                response.Message = "Không tìm thấy";
                response.Data = null;

                Console.WriteLine(ex.ToString());
            }

            return Ok(response);
        }
        
        [HttpGet]
        [Route("api/customer-orders/page")]
        public IHttpActionResult Getcustomer_ordersPaging(int pageSize, int pageNumber)
        {
            ResponseDataDTO<PagedResults<customerorderviewmodel>> response = new ResponseDataDTO<PagedResults<customerorderviewmodel>>();
            try
            {
                response.Code = HttpCode.OK;
                response.Message = MessageResponse.SUCCESS;
                response.Data = _customer_orderservice.CreatePagedResults(pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                response.Message = ex.Message;
                response.Data = null;

                Console.WriteLine(ex.ToString());
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("api/customer-orders/get_staffs_free")]
        public IHttpActionResult GetStaffFree(service_time c,string fullName)
        {
            ResponseDataDTO<List<dropdown>> response = new ResponseDataDTO<List<dropdown>>();
            try
            {
                var results = GenDateOrderService.Gen(c.st_custom_start, c.st_custom_end, c.st_repeat_type, c.st_repeat_every, c.st_sun_flag, c.st_mon_flag, c.st_tue_flag, c.st_wed_flag, c.st_thu_flag, c.st_fri_flag, c.st_sat_flag, c.st_on_day_flag, c.st_on_day, c.st_on_the_flag, c.st_on_the);

                response.Code = HttpCode.OK;
                response.Message = MessageResponse.SUCCESS;
                response.Data = _customer_orderservice.Get_staff_free(results, fullName);
            }
            catch (Exception ex)
            {
                response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                response.Message = ex.Message;
                response.Data = null;

                Console.WriteLine(ex.ToString());
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("api/customer-orders/create")]
        public async Task<IHttpActionResult> CreateCustomerOrderProduct([FromBody] CustomerOrderProductViewModel customer_order)
        {
            ResponseDataDTO<customer_order> response = new ResponseDataDTO<customer_order>();
            try
            {
                var c = customer_order;
                //Id user now
               
                var current_id = BaseController.get_id_current();
                if (c.customer.cu_id == 0)
                {
                    #region[Create Customer]
                    //Cach truong bat buoc 
                    if (c.customer.cu_fullname == null)
                    {
                        response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                        response.Message = "Họ và tên không được để trống";
                        response.Data = null;
                        return Ok(response);
                    }
                    if (c.customer.cu_mobile == null)
                    {
                        response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                        response.Message = "Số điện thoại không được để trống";
                        response.Data = null;
                        return Ok(response);
                    }
                    if (c.customer.cu_email == null)
                    {
                        response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                        response.Message = "Email không được để trống";
                        response.Data = null;
                        return Ok(response);
                    }
                    
                    if (c.customer.cu_type == 0)
                    {
                        response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                        response.Message = "Loại khách hàng không được để trống";
                        response.Data = null;
                        return Ok(response);
                    }
                    if (c.customer.customer_group_id == 0)
                    {
                        response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                        response.Message = "Nhóm khách hàng không được để trống";
                        response.Data = null;
                        return Ok(response);
                    }
                    if (c.customer.source_id == 0)
                    {
                        response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                        response.Message = "Nguồn không được để trống";
                        response.Data = null;
                        return Ok(response);
                    }
                    // get data from formdata
                    CustomerCreateViewModel customerCreateViewModel = new CustomerCreateViewModel
                    {
                        cu_mobile = Convert.ToString(c.customer.cu_mobile),
                        cu_email = Convert.ToString(c.customer.cu_email),
                        cu_fullname = Convert.ToString(c.customer.cu_fullname),

                        customer_group_id = Convert.ToInt32(c.customer.customer_group_id),
                        source_id = Convert.ToInt32(c.customer.source_id),

                        cu_type = Convert.ToByte(c.customer.cu_type),

                    };
                    //Bat cac dieu kien rang buoc
                    if (CheckEmail.IsValidEmail(customerCreateViewModel.cu_email) == false && customerCreateViewModel.cu_email == "")
                    {
                        response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                        response.Message = "Định dạng email không hợp lệ !";
                        response.Data = null;
                        return Ok(response);
                    }

                    if (CheckNumber.IsPhoneNumber(customerCreateViewModel.cu_mobile) == false && customerCreateViewModel.cu_mobile == "")
                    {
                        response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                        response.Message = "Số điện thoại không hợp lệ";
                        response.Data = null;
                        return Ok(response);
                    }


                    //bat cac truog con lai 
                    if (c.customer.cu_birthday == null)
                    {
                        customerCreateViewModel.cu_birthday = null;
                    }
                    else
                    {
                        customerCreateViewModel.cu_birthday = Convert.ToDateTime(c.customer.cu_birthday);
                    }
                    if (c.customer.cu_address == null)
                    {
                        customerCreateViewModel.cu_address = null;
                    }
                    else
                    {
                        customerCreateViewModel.cu_address = Convert.ToString(c.customer.cu_address);
                    }
                    if (c.customer.cu_note == null)
                    {
                        customerCreateViewModel.cu_note = null;
                    }
                    else
                    {
                        customerCreateViewModel.cu_note = Convert.ToString(c.customer.cu_note);
                    }
                    if (c.customer.cu_geocoding == null)
                    {
                        customerCreateViewModel.cu_geocoding = null;
                    }
                    else
                    {
                        customerCreateViewModel.cu_geocoding = Convert.ToString(c.customer.cu_geocoding);
                    }
                    if (c.customer.cu_curator_id == 0)
                    {
                        customerCreateViewModel.cu_curator_id = null;
                    }
                    else
                    {
                        customerCreateViewModel.cu_curator_id = Convert.ToInt32(c.customer.cu_curator_id);
                    }
                    if (c.customer.cu_age == 0)
                    {
                        customerCreateViewModel.cu_age = null;
                    }
                    else
                    {
                        customerCreateViewModel.cu_age = Convert.ToInt32(c.customer.cu_age);
                    }
                    if (c.customer.cu_status == 0)
                    {
                        customerCreateViewModel.cu_status = null;
                    }
                    else
                    {
                        customerCreateViewModel.cu_status = Convert.ToByte(c.customer.cu_status);
                    }
                    
                    customerCreateViewModel.staff_id = Convert.ToInt32(current_id);
                    customerCreateViewModel.cu_create_date = DateTime.Now;
                    var cu = _customerservice.GetLast();
                    if(cu == null) customerCreateViewModel.cu_code = Utilis.CreateCode("CU", 0, 7);
                    else customerCreateViewModel.cu_code = Utilis.CreateCode("CU", cu.cu_id, 7);
                    // mapping view model to entity
                    var createdcustomer = _mapper.Map<customer>(customerCreateViewModel);

                    // save new customer
                    _customerservice.Create(createdcustomer);
                    var cu_last = _customerservice.GetLast();
                    c.customer.cu_id = cu_last.cu_id;

                    // Them dia chi 
                    foreach (shipaddressviewmodel s in c.customer.list_address)
                    {
                        var addresscreate = _mapper.Map<ship_address>(s);
                        addresscreate.customer_id = c.customer.cu_id;
                        _shipaddressservice.Create(addresscreate);
                    }
                    var add_last = _shipaddressservice.GetLast();
                    c.sha_id = add_last.sha_id;
                    #endregion
                }
                

                // get data from formdata
                CustomerOrderCreateViewModel customer_orderCreateViewModel = new CustomerOrderCreateViewModel { };
                customer_orderCreateViewModel.customer_id = c.customer.cu_id;
                customer_orderCreateViewModel.staff_id = Convert.ToInt32(current_id);
                customer_orderCreateViewModel.cuo_payment_status = c.cuo_payment_status;
                customer_orderCreateViewModel.cuo_payment_type = c.cuo_payment_type;
                customer_orderCreateViewModel.cuo_ship_tax = c.cuo_ship_tax;
                customer_orderCreateViewModel.cuo_total_price = c.cuo_total_price;
                customer_orderCreateViewModel.cuo_discount = c.cuo_discount;
                customer_orderCreateViewModel.cuo_status = c.cuo_status;
                customer_orderCreateViewModel.cuo_address = c.cuo_address;
               

                customer_orderCreateViewModel.cuo_date = DateTime.Now;
                // mapping view model to entity
                var createdcustomer_order = _mapper.Map<customer_order>(customer_orderCreateViewModel);
                var op_last1 = _customer_orderservice.GetLast();
                if(op_last1 == null) createdcustomer_order.cuo_code = Utilis.CreateCode("ORP", 0, 7);
                else createdcustomer_order.cuo_code = Utilis.CreateCode("ORP", op_last1.cuo_id,7) ;

                // save new customer_order
                _customer_orderservice.Create(createdcustomer_order);
                var op_last = _customer_orderservice.GetLast();
                //create order product

                foreach (productorderviewmodel i in c.list_product)
                {
                    OrderProductCreateViewModel orderCreateViewModel = new OrderProductCreateViewModel { };
                    orderCreateViewModel.customer_order_id = op_last.cuo_id;
                    orderCreateViewModel.op_discount = i.op_discount;
                    orderCreateViewModel.op_note = i.op_note;
                    orderCreateViewModel.op_quantity = i.op_quantity;
                    orderCreateViewModel.product_id = i.product_id;
                    orderCreateViewModel.op_total_value = i.op_total_value;

                    var createdorderproduct = _mapper.Map<order_product>(orderCreateViewModel);

                    _order_productservice.Create(createdorderproduct);
                }


                // return response
                response.Code = HttpCode.OK;
                response.Message = MessageResponse.SUCCESS;
                response.Data = null;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                response.Message = ex.Message;
                response.Data = null;
                Console.WriteLine(ex.ToString());

                return Ok(response);
            }

        }
        [HttpPost]
        [Route("api/customer-order-service/create")]
        public async Task<IHttpActionResult> CreateCustomerOrderService([FromBody] CustomerOrderServiceViewModelCreate customer_order)
        {
            ResponseDataDTO<customer_order> response = new ResponseDataDTO<customer_order>();
            try
            {
                var c = customer_order;
                //Id user now

                var current_id = BaseController.get_id_current();
                if (c.customer.cu_id == 0)
                {
                    #region[Create Customer]
                    //Cach truong bat buoc 
                    if (c.customer.cu_fullname == null)
                    {
                        response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                        response.Message = "Họ và tên không được để trống";
                        response.Data = null;
                        return Ok(response);
                    }
                    if (c.customer.cu_mobile == null)
                    {
                        response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                        response.Message = "Số điện thoại không được để trống";
                        response.Data = null;
                        return Ok(response);
                    }
                    if (c.customer.cu_email == null)
                    {
                        response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                        response.Message = "Email không được để trống";
                        response.Data = null;
                        return Ok(response);
                    }
                    if (c.customer.cu_type == 0)
                    {
                        response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                        response.Message = "Loại khách hàng không được để trống";
                        response.Data = null;
                        return Ok(response);
                    }
                    if (c.customer.customer_group_id == 0)
                    {
                        response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                        response.Message = "Nhóm khách hàng không được để trống";
                        response.Data = null;
                        return Ok(response);
                    }
                    if (c.customer.source_id == 0)
                    {
                        response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                        response.Message = "Nguồn không được để trống";
                        response.Data = null;
                        return Ok(response);
                    }
                    // get data from formdata
                    CustomerCreateViewModel customerCreateViewModel = new CustomerCreateViewModel
                    {
                        cu_mobile = Convert.ToString(c.customer.cu_mobile),
                        cu_email = Convert.ToString(c.customer.cu_email),
                        cu_fullname = Convert.ToString(c.customer.cu_fullname),

                        customer_group_id = Convert.ToInt32(c.customer.customer_group_id),
                        source_id = Convert.ToInt32(c.customer.source_id),

                        cu_type = Convert.ToByte(c.customer.cu_type),

                    };
                    //Bat cac dieu kien rang buoc
                    if (CheckEmail.IsValidEmail(customerCreateViewModel.cu_email) == false && customerCreateViewModel.cu_email == "")
                    {
                        response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                        response.Message = "Định dạng email không hợp lệ !";
                        response.Data = null;
                        return Ok(response);
                    }

                    if (CheckNumber.IsPhoneNumber(customerCreateViewModel.cu_mobile) == false && customerCreateViewModel.cu_mobile == "")
                    {
                        response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                        response.Message = "Số điện thoại không hợp lệ";
                        response.Data = null;
                        return Ok(response);
                    }


                    //bat cac truog con lai 
                    if (c.customer.cu_birthday == null)
                    {
                        customerCreateViewModel.cu_birthday = null;
                    }
                    else
                    {
                        customerCreateViewModel.cu_birthday = Convert.ToDateTime(c.customer.cu_birthday);
                    }
                    if (c.customer.cu_address == null)
                    {
                        customerCreateViewModel.cu_address = null;
                    }
                    else
                    {
                        customerCreateViewModel.cu_address = Convert.ToString(c.customer.cu_address);
                    }
                    if (c.customer.cu_note == null)
                    {
                        customerCreateViewModel.cu_note = null;
                    }
                    else
                    {
                        customerCreateViewModel.cu_note = Convert.ToString(c.customer.cu_note);
                    }
                    if (c.customer.cu_geocoding == null)
                    {
                        customerCreateViewModel.cu_geocoding = null;
                    }
                    else
                    {
                        customerCreateViewModel.cu_geocoding = Convert.ToString(c.customer.cu_geocoding);
                    }
                    if (c.customer.cu_curator_id == 0)
                    {
                        customerCreateViewModel.cu_curator_id = null;
                    }
                    else
                    {
                        customerCreateViewModel.cu_curator_id = Convert.ToInt32(c.customer.cu_curator_id);
                    }
                    if (c.customer.cu_age == 0)
                    {
                        customerCreateViewModel.cu_age = null;
                    }
                    else
                    {
                        customerCreateViewModel.cu_age = Convert.ToInt32(c.customer.cu_age);
                    }
                    if (c.customer.cu_status == 0)
                    {
                        customerCreateViewModel.cu_status = null;
                    }
                    else
                    {
                        customerCreateViewModel.cu_status = Convert.ToByte(c.customer.cu_status);
                    }

                    customerCreateViewModel.staff_id = Convert.ToInt32(current_id);
                    customerCreateViewModel.cu_create_date = DateTime.Now;
                    var cu = _customerservice.GetLast();
                    if (cu == null) customerCreateViewModel.cu_code = Utilis.CreateCode("CU", 0, 7);
                    else customerCreateViewModel.cu_code = Utilis.CreateCode("CU", cu.cu_id, 7);
                    // mapping view model to entity
                    var createdcustomer = _mapper.Map<customer>(customerCreateViewModel);

                    // save new customer
                    _customerservice.Create(createdcustomer);
                    var cu_last = _customerservice.GetLast();
                    c.customer.cu_id = cu_last.cu_id;

                    // Them dia chi 
                    foreach (shipaddressviewmodel s in c.customer.list_address)
                    {
                        var addresscreate = _mapper.Map<ship_address>(s);
                        addresscreate.customer_id = c.customer.cu_id;
                        _shipaddressservice.Create(addresscreate);
                    }
                    #endregion
                }

                #region create customer order service
                CustomerOrderCreateViewModel customerOrderCreateViewModel = new CustomerOrderCreateViewModel { };
                customerOrderCreateViewModel.customer_id = c.customer.cu_id;
                customerOrderCreateViewModel.staff_id = Convert.ToInt32(current_id);
                customerOrderCreateViewModel.cuo_evaluation = c.cuo_evaluation;
                customerOrderCreateViewModel.cuo_feedback = c.cuo_feedback;
                customerOrderCreateViewModel.cuo_date = DateTime.Now;
                customerOrderCreateViewModel.cuo_address = c.cuo_address;
                customerOrderCreateViewModel.cuo_infor_time = c.cuo_infor_time;
                //Them dia chi shipaddress
                //Delete ship_address old
                List<ship_address> list_ship = _shipaddressservice.GetAllIncluing(x => x.customer_id == c.customer.cu_id).ToList();
                foreach(ship_address sa in list_ship)
                {
                    _shipaddressservice.Delete(sa);
                }
                //Add list shipaddress new
                foreach(shipaddressviewmodel sav in c.customer.list_address)
                {
                    var createShipAddress = _mapper.Map<ship_address>(sav);
                    createShipAddress.customer_id = c.customer.cu_id;
                    _shipaddressservice.Create(createShipAddress);
                }

                // mapping view model to entity
                var createdcustomer_order = _mapper.Map<customer_order>(customerOrderCreateViewModel);
                var op_last1 = _customer_orderservice.GetLast();
                if (op_last1 == null) createdcustomer_order.cuo_code = Utilis.CreateCode("ORS", 0, 7);
                else createdcustomer_order.cuo_code = Utilis.CreateCode("ORS", op_last1.cuo_id, 7);

                // save new customer_order
                _customer_orderservice.Create(createdcustomer_order);

                #endregion
                var op_last = _customer_orderservice.GetLast();
                #region create order service

                for (int i = 0; i < c.list_service_id.Length; i++)
                {
                    OrderServiceCreateViewModel orderCreateViewModel = new OrderServiceCreateViewModel { };
                    orderCreateViewModel.customer_order_id = op_last.cuo_id;
                    orderCreateViewModel.service_id = c.list_service_id[i];
                    var createOrderSecvice = _mapper.Map<order_service>(orderCreateViewModel);
                    _orderserviceservice.Create(createOrderSecvice);

                }
                #endregion
                #region create service time
                ServiceTimeCreateViewModel serviceTimeCreate = new ServiceTimeCreateViewModel();
                serviceTimeCreate.customer_order_id = op_last.cuo_id;
                serviceTimeCreate.st_start_time = c.st_start_time;
                serviceTimeCreate.st_end_time = c.st_end_time;
                serviceTimeCreate.st_start_date = c.st_start_date;
                serviceTimeCreate.st_end_date = c.st_end_date;
                serviceTimeCreate.st_repeat_type = c.st_repeat_type;
                serviceTimeCreate.st_sun_flag = c.st_sun_flag;
                serviceTimeCreate.st_mon_flag = c.st_mon_flag;
                serviceTimeCreate.st_tue_flag = c.st_tue_flag;
                serviceTimeCreate.st_wed_flag = c.st_wed_flag;
                serviceTimeCreate.st_thu_flag = c.st_thu_flag;
                serviceTimeCreate.st_fri_flag = c.st_fri_flag;
                serviceTimeCreate.st_sat_flag = c.st_sat_flag;
                serviceTimeCreate.st_repeat = c.st_repeat;
                serviceTimeCreate.st_repeat_every = c.st_repeat_every;
                serviceTimeCreate.st_on_the = c.st_on_the;
                serviceTimeCreate.st_on_day_flag = c.st_on_day_flag;
                serviceTimeCreate.st_on_day = c.st_on_day;
                serviceTimeCreate.st_on_the_flag = c.st_on_the_flag;
                serviceTimeCreate.st_custom_start = c.st_custom_start;
                if(c.st_custom_end == null)
                    serviceTimeCreate.st_custom_end = c.st_custom_start;
                else
                    serviceTimeCreate.st_custom_end = c.st_custom_end;

                var createServiceTime = _mapper.Map<service_time>(serviceTimeCreate);
                _servicetimeservice.Create(createServiceTime);

                //Do something gen data 
                var st_last = _servicetimeservice.GetLast();
                List<DateTime> results = new List<DateTime>();
                if (c.st_repeat == true)
                {
                    results = GenDateOrderService.Gen(c.st_custom_start, c.st_custom_end, c.st_repeat_type, c.st_repeat_every, c.st_sun_flag , c.st_mon_flag , c.st_tue_flag, c.st_wed_flag, c.st_thu_flag , c.st_fri_flag, c.st_sat_flag, c.st_on_day_flag, c.st_on_day, c.st_on_the_flag, c.st_on_the);
                }
                else
                {
                    for (int i = 0; i < c.list_staff_id.Length; i++)
                    {
                        ExecutorCreateViewModel executorCreateViewModel = new ExecutorCreateViewModel { };
                        executorCreateViewModel.customer_order_id = op_last.cuo_id;
                        executorCreateViewModel.staff_id = c.list_staff_id[i];
                        executorCreateViewModel.service_time_id = st_last.st_id;
                        executorCreateViewModel.work_time = st_last.st_start_date.Date;
                        executorCreateViewModel.start_time = c.st_start_time;
                        executorCreateViewModel.end_time = c.st_end_time;
                        var createExecutor = _mapper.Map<executor>(executorCreateViewModel);
                        _executorservice.Create(createExecutor);

                    }
                }
                #endregion
                
                #region create executor

                for (int i = 0; i < c.list_staff_id.Length; i++)
                {
                    for(int j =0; j< results.Count;j++)
                    {
                        ExecutorCreateViewModel executorCreateViewModel = new ExecutorCreateViewModel { };
                        executorCreateViewModel.customer_order_id = op_last.cuo_id;
                        executorCreateViewModel.staff_id = c.list_staff_id[i];
                        executorCreateViewModel.service_time_id = st_last.st_id;
                        executorCreateViewModel.work_time = results[j].Date;
                        executorCreateViewModel.start_time = c.st_start_time;
                        executorCreateViewModel.end_time = c.st_end_time;
                        var createExecutor = _mapper.Map<executor>(executorCreateViewModel);
                        _executorservice.Create(createExecutor);
                    }
                    
                }
                #endregion

                // return response
                response.Code = HttpCode.OK;
                response.Message = MessageResponse.SUCCESS;
                response.Data = null;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                response.Message = ex.Message;
                response.Data = null;
                Console.WriteLine(ex.ToString());

                return Ok(response);
            }

        }
        [HttpPut]
        [Route("api/customer-order-service/update")]
        public async Task<IHttpActionResult> UpdateCustomerOrderService([FromBody] CustomerOrderServiceViewModelUpdate customer_order)
        {
            ResponseDataDTO<customer_order> response = new ResponseDataDTO<customer_order>();
            try
            {
               
                var c = customer_order;
                //Delete customer order service old 
                var cuoDeleted = _customer_orderservice.Find(c.cuo_id);
                if (cuoDeleted != null)
                {
                    _customer_orderservice.Delete(cuoDeleted);
                }
                //Id user now

                var current_id = BaseController.get_id_current();
                if (c.customer.cu_id == 0)
                {
                    #region[Create Customer]
                    //Cach truong bat buoc 
                    if (c.customer.cu_fullname == null)
                    {
                        response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                        response.Message = "Họ và tên không được để trống";
                        response.Data = null;
                        return Ok(response);
                    }
                    if (c.customer.cu_mobile == null)
                    {
                        response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                        response.Message = "Số điện thoại không được để trống";
                        response.Data = null;
                        return Ok(response);
                    }
                    if (c.customer.cu_email == null)
                    {
                        response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                        response.Message = "Email không được để trống";
                        response.Data = null;
                        return Ok(response);
                    }
                    if (c.customer.cu_type == 0)
                    {
                        response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                        response.Message = "Loại khách hàng không được để trống";
                        response.Data = null;
                        return Ok(response);
                    }
                    if (c.customer.customer_group_id == 0)
                    {
                        response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                        response.Message = "Nhóm khách hàng không được để trống";
                        response.Data = null;
                        return Ok(response);
                    }
                    if (c.customer.source_id == 0)
                    {
                        response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                        response.Message = "Nguồn không được để trống";
                        response.Data = null;
                        return Ok(response);
                    }
                    // get data from formdata
                    CustomerCreateViewModel customerCreateViewModel = new CustomerCreateViewModel
                    {
                        cu_mobile = Convert.ToString(c.customer.cu_mobile),
                        cu_email = Convert.ToString(c.customer.cu_email),
                        cu_fullname = Convert.ToString(c.customer.cu_fullname),

                        customer_group_id = Convert.ToInt32(c.customer.customer_group_id),
                        source_id = Convert.ToInt32(c.customer.source_id),

                        cu_type = Convert.ToByte(c.customer.cu_type),

                    };
                    //Bat cac dieu kien rang buoc
                    if (CheckEmail.IsValidEmail(customerCreateViewModel.cu_email) == false && customerCreateViewModel.cu_email == "")
                    {
                        response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                        response.Message = "Định dạng email không hợp lệ !";
                        response.Data = null;
                        return Ok(response);
                    }

                    if (CheckNumber.IsPhoneNumber(customerCreateViewModel.cu_mobile) == false && customerCreateViewModel.cu_mobile == "")
                    {
                        response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                        response.Message = "Số điện thoại không hợp lệ";
                        response.Data = null;
                        return Ok(response);
                    }


                    //bat cac truog con lai 
                    if (c.customer.cu_birthday == null)
                    {
                        customerCreateViewModel.cu_birthday = null;
                    }
                    else
                    {
                        customerCreateViewModel.cu_birthday = Convert.ToDateTime(c.customer.cu_birthday);
                    }
                    if (c.customer.cu_address == null)
                    {
                        customerCreateViewModel.cu_address = null;
                    }
                    else
                    {
                        customerCreateViewModel.cu_address = Convert.ToString(c.customer.cu_address);
                    }
                    if (c.customer.cu_note == null)
                    {
                        customerCreateViewModel.cu_note = null;
                    }
                    else
                    {
                        customerCreateViewModel.cu_note = Convert.ToString(c.customer.cu_note);
                    }
                    if (c.customer.cu_geocoding == null)
                    {
                        customerCreateViewModel.cu_geocoding = null;
                    }
                    else
                    {
                        customerCreateViewModel.cu_geocoding = Convert.ToString(c.customer.cu_geocoding);
                    }
                    if (c.customer.cu_curator_id == 0)
                    {
                        customerCreateViewModel.cu_curator_id = null;
                    }
                    else
                    {
                        customerCreateViewModel.cu_curator_id = Convert.ToInt32(c.customer.cu_curator_id);
                    }
                    if (c.customer.cu_age == 0)
                    {
                        customerCreateViewModel.cu_age = null;
                    }
                    else
                    {
                        customerCreateViewModel.cu_age = Convert.ToInt32(c.customer.cu_age);
                    }
                    if (c.customer.cu_status == 0)
                    {
                        customerCreateViewModel.cu_status = null;
                    }
                    else
                    {
                        customerCreateViewModel.cu_status = Convert.ToByte(c.customer.cu_status);
                    }

                    customerCreateViewModel.staff_id = Convert.ToInt32(current_id);
                    customerCreateViewModel.cu_create_date = DateTime.Now;
                    var cu = _customerservice.GetLast();
                    if (cu == null) customerCreateViewModel.cu_code = Utilis.CreateCode("CU", 0, 7);
                    else customerCreateViewModel.cu_code = Utilis.CreateCode("CU", cu.cu_id, 7);
                    // mapping view model to entity
                    var createdcustomer = _mapper.Map<customer>(customerCreateViewModel);

                    // save new customer
                    _customerservice.Create(createdcustomer);
                    var cu_last = _customerservice.GetLast();
                    c.customer.cu_id = cu_last.cu_id;

                    // Them dia chi 
                    foreach (shipaddressviewmodel s in c.customer.list_address)
                    {
                        var addresscreate = _mapper.Map<ship_address>(s);
                        addresscreate.customer_id = c.customer.cu_id;
                        _shipaddressservice.Create(addresscreate);
                    }
                    var add_last = _shipaddressservice.GetLast();
                    #endregion
                }

                #region update customer order service

                CustomerOrderCreateViewModel customerOrderCreateViewModel = new CustomerOrderCreateViewModel { };
                customerOrderCreateViewModel.customer_id = c.customer.cu_id;
                customerOrderCreateViewModel.staff_id = Convert.ToInt32(current_id);
                customerOrderCreateViewModel.cuo_evaluation = c.cuo_evaluation;
                customerOrderCreateViewModel.cuo_feedback = c.cuo_feedback;
                customerOrderCreateViewModel.cuo_date = DateTime.Now;
                customerOrderCreateViewModel.cuo_address = c.cuo_address;
                customerOrderCreateViewModel.cuo_infor_time = c.cuo_infor_time;
                //Them dia chi shipaddress
                //Delete ship_address old
                List<ship_address> list_ship = _shipaddressservice.GetAllIncluing(x => x.customer_id == c.customer.cu_id).ToList();
                foreach (ship_address sa in list_ship)
                {
                    _shipaddressservice.Delete(sa);
                }
                //Add list shipaddress new
                foreach (shipaddressviewmodel sav in c.customer.list_address)
                {
                    var createShipAddress = _mapper.Map<ship_address>(sav);
                    createShipAddress.customer_id = c.customer.cu_id;
                    _shipaddressservice.Create(createShipAddress);
                }

                // mapping view model to entity
                var createdcustomer_order = _mapper.Map<customer_order>(customerOrderCreateViewModel);
                var op_last1 = _customer_orderservice.GetLast();
                if (op_last1 == null) createdcustomer_order.cuo_code = Utilis.CreateCode("ORS", 0, 7);
                else createdcustomer_order.cuo_code = Utilis.CreateCode("ORS", op_last1.cuo_id, 7);

                // save new customer_order
                _customer_orderservice.Create(createdcustomer_order);

                #endregion
                var op_last = _customer_orderservice.GetLast();
                #region create order service

                for (int i = 0; i < c.list_service_id.Length; i++)
                {
                    OrderServiceCreateViewModel orderCreateViewModel = new OrderServiceCreateViewModel { };
                    orderCreateViewModel.customer_order_id = op_last.cuo_id;
                    orderCreateViewModel.service_id = c.list_service_id[i];
                    var createOrderSecvice = _mapper.Map<order_service>(orderCreateViewModel);
                    _orderserviceservice.Create(createOrderSecvice);

                }
                #endregion
                #region create service time
                ServiceTimeCreateViewModel serviceTimeCreate = new ServiceTimeCreateViewModel();
                serviceTimeCreate.customer_order_id = op_last.cuo_id;
                serviceTimeCreate.st_start_time = c.st_start_time;
                serviceTimeCreate.st_end_time = c.st_end_time;
                serviceTimeCreate.st_start_date = c.st_start_date;
                serviceTimeCreate.st_end_date = c.st_end_date;
                serviceTimeCreate.st_repeat_type = c.st_repeat_type;
                serviceTimeCreate.st_sun_flag = c.st_sun_flag;
                serviceTimeCreate.st_mon_flag = c.st_mon_flag;
                serviceTimeCreate.st_tue_flag = c.st_tue_flag;
                serviceTimeCreate.st_wed_flag = c.st_wed_flag;
                serviceTimeCreate.st_thu_flag = c.st_thu_flag;
                serviceTimeCreate.st_fri_flag = c.st_fri_flag;
                serviceTimeCreate.st_sat_flag = c.st_sat_flag;
                serviceTimeCreate.st_repeat = c.st_repeat;
                serviceTimeCreate.st_repeat_every = c.st_repeat_every;
                serviceTimeCreate.st_on_the = c.st_on_the;
                serviceTimeCreate.st_on_day_flag = c.st_on_day_flag;
                serviceTimeCreate.st_on_day = c.st_on_day;
                serviceTimeCreate.st_on_the_flag = c.st_on_the_flag;
                serviceTimeCreate.st_custom_start = c.st_custom_start;
                if (c.st_custom_end == null)
                    serviceTimeCreate.st_custom_end = c.st_custom_start;
                else
                    serviceTimeCreate.st_custom_end = c.st_custom_end;
                var createServiceTime = _mapper.Map<service_time>(serviceTimeCreate);
                _servicetimeservice.Create(createServiceTime);

                //Do something gen data 
                var st_last = _servicetimeservice.GetLast();
                List<DateTime> results = new List<DateTime>();
                if (c.st_repeat == true)
                {
                    results = GenDateOrderService.Gen(c.st_custom_start, c.st_custom_end, c.st_repeat_type, c.st_repeat_every, c.st_sun_flag, c.st_mon_flag, c.st_tue_flag, c.st_wed_flag, c.st_thu_flag, c.st_fri_flag, c.st_sat_flag, c.st_on_day_flag, c.st_on_day, c.st_on_the_flag, c.st_on_the);
                }
                else
                {
                    for (int i = 0; i < c.list_staff_id.Length; i++)
                    {
                        ExecutorCreateViewModel executorCreateViewModel = new ExecutorCreateViewModel { };
                        executorCreateViewModel.customer_order_id = op_last.cuo_id;
                        executorCreateViewModel.staff_id = c.list_staff_id[i];
                        executorCreateViewModel.service_time_id = st_last.st_id;
                        executorCreateViewModel.work_time = st_last.st_start_date.Date;
                        executorCreateViewModel.start_time = c.st_start_time;
                        executorCreateViewModel.end_time = c.st_end_time;
                        var createExecutor = _mapper.Map<executor>(executorCreateViewModel);
                        _executorservice.Create(createExecutor);

                    }
                }
                #endregion
                
                #region create executor

                for (int i = 0; i < c.list_staff_id.Length; i++)
                {
                    for (int j = 0; j < results.Count; j++)
                    {
                        ExecutorCreateViewModel executorCreateViewModel = new ExecutorCreateViewModel { };
                        executorCreateViewModel.customer_order_id = op_last.cuo_id;
                        executorCreateViewModel.staff_id = c.list_staff_id[i];
                        executorCreateViewModel.service_time_id = st_last.st_id;
                        executorCreateViewModel.work_time = results[j].Date;
                        executorCreateViewModel.start_time = c.st_start_time;
                        executorCreateViewModel.end_time = c.st_end_time;
                        var createExecutor = _mapper.Map<executor>(executorCreateViewModel);
                        _executorservice.Create(createExecutor);
                    }

                }
                #endregion



                // return response
                response.Code = HttpCode.OK;
                response.Message = MessageResponse.SUCCESS;
                response.Data = null;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                response.Message = ex.Message;
                response.Data = null;
                Console.WriteLine(ex.ToString());

                return Ok(response);
            }

        }

        [HttpPut]
        [Route("api/customer-orders/update")]
        public async Task<IHttpActionResult> UpdateCustomerOder([FromBody] CustomerOrderProductViewModelUpdate customer_order_update)
        {
            ResponseDataDTO<bool> response = new ResponseDataDTO<bool>();

            try
            {

                //Id user now
                new BaseController();
                var current_id = BaseController.get_id_current();
                if (customer_order_update.customer.cu_id == 0)
                {
                    #region[Create Customer]
                    //Cach truong bat buoc 
                    if (customer_order_update.customer.cu_fullname == null)
                    {
                        response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                        response.Message = "Họ và tên không được để trống";
                        response.Data = false;
                        return Ok(response);
                    }
                    if (customer_order_update.customer.cu_mobile == null)
                    {
                        response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                        response.Message = "Số điện thoại không được để trống";
                        response.Data = false;
                        return Ok(response);
                    }
                    if (customer_order_update.customer.cu_email == null)
                    {
                        response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                        response.Message = "Email không được để trống";
                        response.Data = false;
                        return Ok(response);
                    }
                    if (customer_order_update.customer.cu_type == 0)
                    {
                        response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                        response.Message = "Loại khách hàng không được để trống";
                        response.Data = false;
                        return Ok(response);
                    }
                    if (customer_order_update.customer.customer_group_id == 0)
                    {
                        response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                        response.Message = "Nhóm khách hàng không được để trống";
                        response.Data = false;
                        return Ok(response);
                    }
                    if (customer_order_update.customer.source_id == 0)
                    {
                        response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                        response.Message = "Nguồn không được để trống";
                        response.Data = false;
                        return Ok(response);
                    }
                    // get data from formdata
                    CustomerCreateViewModel customerCreateViewModel = new CustomerCreateViewModel
                    {
                        cu_mobile = Convert.ToString(customer_order_update.customer.cu_mobile),
                        cu_email = Convert.ToString(customer_order_update.customer.cu_email),
                        cu_fullname = Convert.ToString(customer_order_update.customer.cu_fullname),

                        customer_group_id = Convert.ToInt32(customer_order_update.customer.customer_group_id),
                        source_id = Convert.ToInt32(customer_order_update.customer.source_id),

                        cu_type = Convert.ToByte(customer_order_update.customer.cu_type),

                    };
                    //Bat cac dieu kien rang buoc
                    if (CheckEmail.IsValidEmail(customerCreateViewModel.cu_email) == false && customerCreateViewModel.cu_email == "")
                    {
                        response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                        response.Message = "Định dạng email không hợp lệ !";
                        response.Data = false;
                        return Ok(response);
                    }

                    if (CheckNumber.IsPhoneNumber(customerCreateViewModel.cu_mobile) == false && customerCreateViewModel.cu_mobile == "")
                    {
                        response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                        response.Message = "Số điện thoại không hợp lệ";
                        response.Data = false;
                        return Ok(response);
                    }


                    //bat cac truog con lai 
                    if (customer_order_update.customer.cu_birthday == null)
                    {
                        customerCreateViewModel.cu_birthday = null;
                    }
                    else
                    {
                        customerCreateViewModel.cu_birthday = Convert.ToDateTime(customer_order_update.customer.cu_birthday);
                    }
                    if (customer_order_update.customer.cu_address == null)
                    {
                        customerCreateViewModel.cu_address = null;
                    }
                    else
                    {
                        customerCreateViewModel.cu_address = Convert.ToString(customer_order_update.customer.cu_address);
                    }
                    if (customer_order_update.customer.cu_note == null)
                    {
                        customerCreateViewModel.cu_note = null;
                    }
                    else
                    {
                        customerCreateViewModel.cu_note = Convert.ToString(customer_order_update.customer.cu_note);
                    }
                    if (customer_order_update.customer.cu_geocoding == null)
                    {
                        customerCreateViewModel.cu_geocoding = null;
                    }
                    else
                    {
                        customerCreateViewModel.cu_geocoding = Convert.ToString(customer_order_update.customer.cu_geocoding);
                    }
                    if (customer_order_update.customer.cu_curator_id == 0)
                    {
                        customerCreateViewModel.cu_curator_id = null;
                    }
                    else
                    {
                        customerCreateViewModel.cu_curator_id = Convert.ToInt32(customer_order_update.customer.cu_curator_id);
                    }
                    if (customer_order_update.customer.cu_age == 0)
                    {
                        customerCreateViewModel.cu_age = null;
                    }
                    else
                    {
                        customerCreateViewModel.cu_age = Convert.ToInt32(customer_order_update.customer.cu_age);
                    }
                    if (customer_order_update.customer.cu_status == 0)
                    {
                        customerCreateViewModel.cu_status = null;
                    }
                    else
                    {
                        customerCreateViewModel.cu_status = Convert.ToByte(customer_order_update.customer.cu_status);
                    }

                    customerCreateViewModel.staff_id = Convert.ToInt32(current_id);
                    customerCreateViewModel.cu_create_date = DateTime.Now;
                    var cu = _customerservice.GetLast();
                    if(cu == null) customerCreateViewModel.cu_code = Utilis.CreateCode("CU", 0, 7);
                    else customerCreateViewModel.cu_code = Utilis.CreateCode("CU", cu.cu_id, 7);
                    // mapping view model to entity
                    var createdcustomer = _mapper.Map<customer>(customerCreateViewModel);

                    // save new customer
                    _customerservice.Create(createdcustomer);
                    var cu_last = _customerservice.GetLast();
                    customer_order_update.customer.cu_id = cu_last.cu_id;
                    // Them dia chi 
                    foreach (shipaddressviewmodel s in customer_order_update.customer.list_address)
                    {
                        var addresscreate = _mapper.Map<ship_address>(s);
                        addresscreate.customer_id = customer_order_update.customer.cu_id;
                        _shipaddressservice.Create(addresscreate);
                    }
                    var add_last = _shipaddressservice.GetLast();
                    customer_order_update.sha_id = add_last.sha_id;
                    #endregion
                }


                var existscustomerorder = _customer_orderservice.Find(customer_order_update.cuo_id);

                existscustomerorder.customer_id = customer_order_update.customer.cu_id;
                existscustomerorder.staff_id = Convert.ToInt32(current_id);
                existscustomerorder.cuo_payment_status = customer_order_update.cuo_payment_status;
                existscustomerorder.cuo_payment_type = customer_order_update.cuo_payment_type;
                existscustomerorder.cuo_ship_tax = customer_order_update.cuo_ship_tax;
                existscustomerorder.cuo_total_price = customer_order_update.cuo_total_price;
                existscustomerorder.cuo_discount = customer_order_update.cuo_discount;
                existscustomerorder.cuo_status = customer_order_update.cuo_status;
                existscustomerorder.cuo_address = customer_order_update.cuo_address;

                


                // update customer order
                _customer_orderservice.Update(existscustomerorder, existscustomerorder.cuo_id);
                var list_product_old = _order_productservice.GetAllIncluing(op => op.customer_order_id == existscustomerorder.cuo_id);
                foreach( order_product i in list_product_old)
                {
                    _order_productservice.Delete(i);
                }
                //update order product

                OrderProductCreateViewModel orderCreateViewModel = new OrderProductCreateViewModel { };

                foreach (productorderviewmodel i in customer_order_update.list_product)
                {
                   

                    orderCreateViewModel.customer_order_id = customer_order_update.cuo_id;
                    orderCreateViewModel.op_discount = i.op_discount;
                    orderCreateViewModel.op_note = i.op_note;
                    orderCreateViewModel.op_quantity = i.op_quantity;
                    orderCreateViewModel.product_id = i.product_id;
                    orderCreateViewModel.op_total_value = i.op_total_value;


                    var createdorderproduct = _mapper.Map<order_product>(orderCreateViewModel);

                    _order_productservice.Create(createdorderproduct);
                }


                // return response
                response.Code = HttpCode.OK;
                response.Message = MessageResponse.SUCCESS;
                response.Data = false;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                response.Message = ex.Message;
                response.Data = true;
                Console.WriteLine(ex.ToString());

                return Ok(response);
            }

        }

        [HttpPut]
        [Route("api/customer-orders/update-status")]
        public async Task<IHttpActionResult> UpdateStatusCustomerOrder()
        {
            ResponseDataDTO<customer_order> response = new ResponseDataDTO<customer_order>();
            try
            {
                var path = Path.GetTempPath();

                if (!Request.Content.IsMimeMultipartContent("form-data"))
                {
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.UnsupportedMediaType));
                }

                MultipartFormDataStreamProvider streamProvider = new MultipartFormDataStreamProvider(path);

                await Request.Content.ReadAsMultipartAsync(streamProvider);
                int cuo_id = Convert.ToInt32(streamProvider.FormData["cuo_id"]);
                var cuo_update = _customer_orderservice.Find(cuo_id);
                cuo_update.cuo_status = Convert.ToByte(streamProvider.FormData["cuo_status"]);
                // update address
                _customer_orderservice.Update(cuo_update, cuo_id);
                // return response
                response.Code = HttpCode.OK;
                response.Message = MessageResponse.SUCCESS;
                response.Data = cuo_update;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                response.Message = ex.Message;
                response.Data = null;
                Console.WriteLine(ex.ToString());

                return Ok(response);
            }
        }

        [HttpDelete]
        [Route("api/customer_orders/delete")]
        public IHttpActionResult Deletecustomer_order(int customer_orderId)
        {
            ResponseDataDTO<customer_order> response = new ResponseDataDTO<customer_order>();
            try
            {
                var customer_orderDeleted = _customer_orderservice.Find(customer_orderId);
                if (customer_orderDeleted != null)
                {
                    _customer_orderservice.Delete(customer_orderDeleted);

                    // return response
                    response.Code = HttpCode.OK;
                    response.Message = MessageResponse.SUCCESS;
                    response.Data = null;
                    return Ok(response);
                }
                else
                {
                    // return response
                    response.Code = HttpCode.NOT_FOUND;
                    response.Message = "Không tìm thấy mã khách hàng order trong hệ thống.";
                    response.Data = null;

                    return Ok(response);
                }


            }
            catch (Exception ex)
            {
                response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                response.Message = ex.Message;
                response.Data = null;
                Console.WriteLine(ex.ToString());

                return Ok(response);
            }
        }
        #endregion

        #region["Dịch vụ"]
        [HttpGet]
        [Route("api/customer-orders/service_by_date")]
        public IHttpActionResult GetServiceByDay(DateTime start_date, DateTime to_date)
        {
            ResponseDataDTO<List<order_service_view>> response = new ResponseDataDTO<List<order_service_view>>();
            try
            {
                int staff_id = BaseController.get_id_current();
                response.Code = HttpCode.OK;
                response.Message = MessageResponse.SUCCESS;
                response.Data = _customer_orderservice.GetServiceByDay(staff_id,start_date, to_date);
            }
            catch (Exception ex)
            {
                response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                response.Message = ex.Message;
                response.Data = null;

                Console.WriteLine(ex.ToString());
            }

            return Ok(response);
        }
        #endregion

        #region["Export Excel"]
        [HttpGet]
        [Route("api/customer-order/export")]
        public async Task<IHttpActionResult> ExportCustomerOrder(int pageNumber, int pageSize, int? payment_type_id, DateTime? start_date, DateTime? end_date, string name)
        {
            ResponseDataDTO<string> response = new ResponseDataDTO<string>();
            try
            {
                var list_customer_order = new List<customerorderview>();

                //Đưa ra danh sách staff trong trang nào đó 
                var objRT_Mst_Customer_Order = _customer_orderservice.ExportCustomerOrder(pageNumber, pageSize, payment_type_id,start_date,end_date, name);
                if (objRT_Mst_Customer_Order != null)
                {
                    list_customer_order.AddRange(objRT_Mst_Customer_Order.Results);

                    Dictionary<string, string> dicColNames = GetImportDicColums();

                    string url = "";
                    string filePath = GenExcelExportFilePath(string.Format(typeof(customer_order).Name), ref url);

                    ExcelExport.ExportToExcelFromList(list_customer_order, dicColNames, filePath, string.Format("Đặt hàng"));
                    //Input: http://27.72.147.222:1230/TempFiles/2020-03-11/department_200311210940.xlsx
                    //"D:\\BootAi\\ERP20\\ERP.API\\TempFiles\\2020-03-12\\department_200312092643.xlsx"

                    filePath = filePath.Replace("\\", "/");
                    int index = filePath.IndexOf("TempFiles");
                    filePath = filePath.Substring(index);
                    response.Code = HttpCode.OK;
                    response.Message = "Đã xuất excel thành công!";
                    response.Data = filePath;
                }
                else
                {
                    response.Code = HttpCode.NOT_FOUND;
                    response.Message = "File excel import không có dữ liệu!";
                    response.Data = null;
                }
            }
            catch (Exception ex)
            {
                response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                response.Message = ex.Message; ;
                response.Data = null;
                Console.WriteLine(ex.ToString());

                return Ok(response);
            }
            return Ok(response);
        }
        #endregion

        #region["DicColums"]
        private Dictionary<string, string> GetImportDicColums()
        {
            return new Dictionary<string, string>()
            {

                 {"cuo_code","MDH" },
                 {"cuo_date","Ngày tạo"},
                 {"cuo_total_price","Tổng tiền"},
                 {"cuo_status_name","Trạng thái đơn hàng"},
                 {"customer_name","Khách hàng"},
                 {"cuo_payment_type_name","Loại thanh toán"},
                 {"cuo_payment_status_name","Trạng thái thanh toán"},
                 {"cuo_ship_tax","Phí vận chuyển"},
                 {"staff_name","Người tạo đơn"},
                 {"cuo_address","Địa chỉ"},
                 {"cuo_note","Chú ý"}
                 
                 
            };
        }
        private Dictionary<string, string> GetImportDicColumsTemplate()
        {
            return new Dictionary<string, string>()
            {
                  {"email","Email phong ban"},
                 {"id","Ma bộ phận phòng ban"}
            };
        }
        #endregion

        #region dispose

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _customer_orderservice.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}