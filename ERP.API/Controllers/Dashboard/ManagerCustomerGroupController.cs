﻿using AutoMapper;
using ERP.API.Models;
using ERP.Common.Constants;
using ERP.Common.Models;
using ERP.Data.Dto;
using ERP.Data.ModelsERP;
using ERP.Data.ModelsERP.ModelView;
using ERP.Extension.Extensions;
using ERP.Service.Services.IServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ERP.API.Controllers.Dashboard
{
    [EnableCors("*", "*", "*")]
    [Authorize]
    public class ManagerCustomerGroupController : ApiController
    {
        private readonly ICustomerGroupService _customer_groupservice;

        private readonly IMapper _mapper;

        public ManagerCustomerGroupController() { }
        public ManagerCustomerGroupController(ICustomerGroupService customer_groupservice, IMapper mapper)
        {
            this._customer_groupservice = customer_groupservice;
            this._mapper = mapper;
        }

        #region methods
        [HttpGet]
        [Route("api/customer-groups/search")]
        public IHttpActionResult Getcustomer_groupsPaging(int pageSize, int pageNumber, int? cg_id, string name)
        {
            ResponseDataDTO<PagedResults<customergroupviewmodel>> response = new ResponseDataDTO<PagedResults<customergroupviewmodel>>();
            try
            {
                response.Code = HttpCode.OK;
                response.Message = MessageResponse.SUCCESS;
                response.Data = _customer_groupservice.GetAllPageSearch(pageNumber, pageSize, cg_id, name);
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
        [Route("api/customer-groups/all")]
        public IHttpActionResult GetAll()
        {
            ResponseDataDTO<IEnumerable<customer_group>> response = new ResponseDataDTO<IEnumerable<customer_group>>();
            try
            {
                response.Code = HttpCode.OK;
                response.Message = MessageResponse.SUCCESS;
                response.Data = _customer_groupservice.GetAll();
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
        #region [Create]
        [HttpPost]
        [Route("api/customer-group/create")]

        public async Task<IHttpActionResult> CreateCustomerGroup()
        {
            ResponseDataDTO<customer_group> response = new ResponseDataDTO<customer_group>();
            try
            {
                var path = Path.GetTempPath();

                if (!Request.Content.IsMimeMultipartContent("form-data"))
                {
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.UnsupportedMediaType));
                }
;
                MultipartFormDataStreamProvider streamProvider = new MultipartFormDataStreamProvider(path);

                await Request.Content.ReadAsMultipartAsync(streamProvider);
                string fileName = "";
                foreach (MultipartFileData fileData in streamProvider.FileData)
                {
                    fileName = (FileExtension.SaveFileOnDisk(fileData));
                }
                //Các trường bắt buộc 
                if (streamProvider.FormData["cg_name"] == null)
                {
                    response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                    response.Message = "Tên nhóm không được để trống";
                    response.Data = null;
                    return Ok(response);
                }




                // get data from formdata
                CustomerGroupCreateViewModel customerGroupCreateViewModel = new CustomerGroupCreateViewModel
                {
                    cg_name = Convert.ToString(streamProvider.FormData["cg_name"]),

                };
                if (streamProvider.FormData["cg_description"] == null)
                {
                    customerGroupCreateViewModel.cg_description = null;
                }
                else
                {
                    customerGroupCreateViewModel.cg_description = Convert.ToString(streamProvider.FormData["cg_description"]);
                }

                //Create date
                customerGroupCreateViewModel.cg_created_date = DateTime.Now;
                customerGroupCreateViewModel.staff_id = BaseController.get_id_current();

                // mapping view model to entity
                var create_customer_group = _mapper.Map<customer_group>(customerGroupCreateViewModel);
                // save file

                if (fileName == null)
                {
                    create_customer_group.cg_thumbnail = "/Uploads/Images/default/product.png";
                }
                else create_customer_group.cg_thumbnail = fileName;

                // save new product
                _customer_groupservice.Create(create_customer_group);
                // return response
                response.Code = HttpCode.OK;
                response.Message = MessageResponse.SUCCESS;
                response.Data = create_customer_group;
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
        #endregion
        #region [Create]
        [HttpPost]
        [Route("api/customer-group/update")]

        public async Task<IHttpActionResult> UpdateCustomerGroup()
        {
            ResponseDataDTO<customer_group> response = new ResponseDataDTO<customer_group>();
            try
            {
                var path = Path.GetTempPath();

                if (!Request.Content.IsMimeMultipartContent("form-data"))
                {
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.UnsupportedMediaType));
                }

                MultipartFormDataStreamProvider streamProvider = new MultipartFormDataStreamProvider(path);

                await Request.Content.ReadAsMultipartAsync(streamProvider);
                string fileName = "";
                foreach (MultipartFileData fileData in streamProvider.FileData)
                {
                    fileName = (FileExtension.SaveFileOnDisk(fileData));
                }
                //Các trường bắt buộc 
                if (streamProvider.FormData["cg_name"] == null)
                {
                    response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                    response.Message = "Tên nhóm không được để trống";
                    response.Data = null;
                    return Ok(response);
                }




                // get data from formdata
                CustomerGroupUpdateViewModel customerGroupUpdateViewModel = new CustomerGroupUpdateViewModel
                {
                    cg_name = Convert.ToString(streamProvider.FormData["cg_name"]),
                    cg_id = Convert.ToInt32(streamProvider.FormData["cg_id"]),


                };
                if (streamProvider.FormData["cg_description"] == null)
                {
                    customerGroupUpdateViewModel.cg_description = null;
                }
                else
                {
                    customerGroupUpdateViewModel.cg_description = Convert.ToString(streamProvider.FormData["cg_description"]);
                }

                //Create date
                customerGroupUpdateViewModel.cg_created_date = DateTime.Now;
                customerGroupUpdateViewModel.staff_id = BaseController.get_id_current();
                var existscg = _customer_groupservice.Find(customerGroupUpdateViewModel.cg_id);

                if (streamProvider.FormData["cu_thumbnail"] != null)
                {
                    if (fileName != "")
                    {
                        customerGroupUpdateViewModel.cg_thumbnail = fileName;
                    }
                    else
                    {

                        customerGroupUpdateViewModel.cg_thumbnail = existscg.cg_thumbnail;
                    }
                }

                // mapping view model to entity
                var update_customer_group = _mapper.Map<customer_group>(customerGroupUpdateViewModel);
                // save file



                // save new product
                _customer_groupservice.Update(update_customer_group, update_customer_group.cg_id);
                // return response
                response.Code = HttpCode.OK;
                response.Message = MessageResponse.SUCCESS;
                response.Data = update_customer_group;
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
        #endregion
        [HttpDelete]
        [Route("api/customer-group/delete")]
        public IHttpActionResult Deletestaff(int staffId)
        {
            ResponseDataDTO<staff> response = new ResponseDataDTO<staff>();
            try
            {
                var staffDeleted = _customer_groupservice.Find(staffId);
                if (staffDeleted != null)
                {
                    _customer_groupservice.Delete(staffDeleted);

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
                    response.Message = MessageResponse.FAIL;
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
        #region dispose

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _customer_groupservice.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}