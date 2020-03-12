﻿using AutoMapper;
using ERP.API.Models;
using ERP.Common.Constants;
using ERP.Common.Excel;
using ERP.Common.Models;
using ERP.Data.Dto;
using ERP.Data.ModelsERP;
using ERP.Data.ModelsERP.ModelView;
using ERP.Data.ModelsERP.ModelView.ExportDB;
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
    public class ManagerProductController : BaseController
    {
        private readonly IProductService _productservice;

        private readonly IMapper _mapper;

        public ManagerProductController() { }
        public ManagerProductController(IProductService productservice, IMapper mapper)
        {
            this._productservice = productservice;
            this._mapper = mapper;
        }

        #region methods
        [HttpGet]
        [Route("api/products/all")]
        public IHttpActionResult Getproducts()
        {
            var current_id = BaseController.get_id_current();
            ResponseDataDTO<IEnumerable<product>> response = new ResponseDataDTO<IEnumerable<product>>();
            try
            {
                response.Code = HttpCode.OK;
                response.Message = MessageResponse.SUCCESS;
                response.Data = _productservice.GetAllIncluing(t => t.pu_quantity == 1, q => q.OrderBy(s => s.pu_id));
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

        [Route("api/products/page")]
        public IHttpActionResult GetproductsPaging(int pageSize, int pageNumber)
        {
            ResponseDataDTO<PagedResults<productviewmodel>> response = new ResponseDataDTO<PagedResults<productviewmodel>>();
            try
            {
                response.Code = HttpCode.OK;
                response.Message = MessageResponse.SUCCESS;
                response.Data = _productservice.GetAllPage(pageNumber, pageSize);
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
        [Route("api/products/unit")]
        public IHttpActionResult GetUnit()
        {
            ResponseDataDTO<List<dropdown>> response = new ResponseDataDTO<List<dropdown>> ();
            try
            {
                response.Code = HttpCode.OK;
                response.Message = MessageResponse.SUCCESS;
                response.Data = _productservice.GetUnit();
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
        #region [Get By Id]
        [Route("api/products/infor")]
        public IHttpActionResult GetAllById(int pu_id)
        {
            ResponseDataDTO<PagedResults<productviewmodel>> response = new ResponseDataDTO<PagedResults<productviewmodel>>();
            try
            {
                response.Code = HttpCode.OK;
                response.Message = MessageResponse.SUCCESS;
                response.Data = _productservice.GetAllPageById(pu_id);
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
        #region[Search]
        [Route("api/products/search")]
        public IHttpActionResult GetProducts(int pageNumber, int pageSize, string search_name, int? category_id)
        {
            ResponseDataDTO<PagedResults<productviewmodel>> response = new ResponseDataDTO<PagedResults<productviewmodel>>();
            try
            {
                response.Code = HttpCode.OK;
                response.Message = MessageResponse.SUCCESS;
                response.Data = _productservice.GetProducts(pageNumber:pageNumber, pageSize:pageSize, search_name:search_name, category_id:category_id);
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
        #endregion

        #region [Create]
        [HttpPost]
        [Route("api/products/create")]

        public async Task<IHttpActionResult> Createproduct()
        {
            ResponseDataDTO<product> response = new ResponseDataDTO<product>();
            try
            {
                var path = Path.GetTempPath();

                if (!Request.Content.IsMimeMultipartContent("form-data"))
                {
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.UnsupportedMediaType));
                }

                MultipartFormDataStreamProvider streamProvider = new MultipartFormDataStreamProvider(path);

                await Request.Content.ReadAsMultipartAsync(streamProvider);
                //Các trường bắt buộc 
                if (streamProvider.FormData["pu_name"] == null)
                {
                    response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                    response.Message = "Tên sản phẩm không được để trống";
                    response.Data = null;
                    return Ok(response);
                }
                if (streamProvider.FormData["pu_quantity"] == null)
                {
                    response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                    response.Message = "Số lượng không được để trống";
                    response.Data = null;
                    return Ok(response);
                }
                if (streamProvider.FormData["pu_buy_price"] == null)
                {
                    response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                    response.Message = "Giá bán không được để trống";
                    response.Data = null;
                    return Ok(response);
                }

                if (streamProvider.FormData["pu_sale_price"] == null)
                {
                    response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                    response.Message = "Giá mua không được để trống";
                    response.Data = null;
                    return Ok(response);
                }
                if (streamProvider.FormData["pu_tax"] == null)
                {
                    response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                    response.Message = "Thuế không được để trống";
                    response.Data = null;
                    return Ok(response);
                }

                if (streamProvider.FormData["pu_unit"] == null)
                {
                    response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                    response.Message = "Đơn vị không được để trống";
                    response.Data = null;
                    return Ok(response);
                }
                if (streamProvider.FormData["product_category_id"] == null)
                {
                    response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                    response.Message = "Loại sản phẩm không được để trống";
                    response.Data = null;
                    return Ok(response);
                }
                if (streamProvider.FormData["provider_id"] == null)
                {
                    response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                    response.Message = "Nhà cung cấp không được để trống";
                    response.Data = null;
                    return Ok(response);
                }


                // get data from formdata
                ProductCreateViewModel productCreateViewModel = new ProductCreateViewModel
                {
                    pu_name = Convert.ToString(streamProvider.FormData["pu_name"]),

                    pu_quantity = Convert.ToInt32(streamProvider.FormData["pu_quantity"]),
                    pu_buy_price = Convert.ToInt32(streamProvider.FormData["pu_buy_price"]),
                    pu_sale_price = Convert.ToInt32(streamProvider.FormData["pu_sale_price"]),
                    
                    product_category_id = Convert.ToInt32(streamProvider.FormData["product_category_id"]),
                    provider_id= Convert.ToInt32(streamProvider.FormData["provider_id"]),
                    pu_tax = Convert.ToInt32(streamProvider.FormData["pu_tax"]),
                    pu_unit = Convert.ToByte(streamProvider.FormData["pu_unit"]),

                };
                //Kiem tra cac truong con lại 
                if (streamProvider.FormData["pu_update_date"] == null)
                {
                    productCreateViewModel.pu_update_date = null;
                }
                else
                {
                    productCreateViewModel.pu_update_date = Convert.ToDateTime(streamProvider.FormData["pu_update_date"]);
                }

                if (streamProvider.FormData["pu_saleoff"] == null)
                {
                    productCreateViewModel.pu_saleoff = null;
                }
                else
                {
                    productCreateViewModel.pu_saleoff = Convert.ToInt32(streamProvider.FormData["pu_saleoff"]);
                }

                if (streamProvider.FormData["pu_weight"] == null)
                {
                    productCreateViewModel.pu_weight = null;
                }
                else
                {
                    productCreateViewModel.pu_weight = Convert.ToInt32(streamProvider.FormData["pu_weight"]);
                }

                if (streamProvider.FormData["pu_description"] == null)
                {
                    productCreateViewModel.pu_description = null;
                }
                else
                {
                    productCreateViewModel.pu_description = Convert.ToString(streamProvider.FormData["pu_description"]);
                }

                if (streamProvider.FormData["pu_short_description"] == null)
                {
                    productCreateViewModel.pu_short_description = null;
                }
                else
                {
                    productCreateViewModel.pu_short_description = Convert.ToString(streamProvider.FormData["pu_short_description"]);
                }
                //Tạo mã 
                var x = _productservice.GetLast();
                if(x == null) productCreateViewModel.pu_code = Utilis.CreateCode("PR", 0, 7);
                else productCreateViewModel.pu_code = Utilis.CreateCode("PR", x.pu_id, 7);
                //Create date
                productCreateViewModel.pu_create_date = DateTime.Now;

                // mapping view model to entity
                var createdproduct = _mapper.Map<product>(productCreateViewModel);
                // save file
                string fileName = "";
                foreach (MultipartFileData fileData in streamProvider.FileData)
                {
                    fileName = (FileExtension.SaveFileProductOnDisk(fileData, createdproduct.pu_code));
                }
                if (fileName == null)
                {
                    createdproduct.pu_thumbnail = "/Uploads/Images/default/product.png";
                }
                else createdproduct.pu_thumbnail = fileName;

                // save new product
                _productservice.Create(createdproduct);
                // return response
                response.Code = HttpCode.OK;
                response.Message = MessageResponse.SUCCESS;
                response.Data = createdproduct;
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

        #region[Update]
        [HttpPut]
        [Route("api/products/update")]

        public async Task<IHttpActionResult> Updateproduct()
        {
            ResponseDataDTO<product> response = new ResponseDataDTO<product>();
            try
            {
                var path = Path.GetTempPath();

                if (!Request.Content.IsMimeMultipartContent("form-data"))
                {
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.UnsupportedMediaType));
                }

                MultipartFormDataStreamProvider streamProvider = new MultipartFormDataStreamProvider(path);

                await Request.Content.ReadAsMultipartAsync(streamProvider);
                //Các trường bắt buộc
                if (streamProvider.FormData["pu_name"] == null)
                {
                    response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                    response.Message = "Tên sản phẩm không được để trống";
                    response.Data = null;
                    return Ok(response);
                }
                if (streamProvider.FormData["pu_quantity"] == null)
                {
                    response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                    response.Message = "Số lượng không được để trống";
                    response.Data = null;
                    return Ok(response);
                }
                if (streamProvider.FormData["pu_buy_price"] == null)
                {
                    response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                    response.Message = "Giá bán không được để trống";
                    response.Data = null;
                    return Ok(response);
                }

                if (streamProvider.FormData["pu_sale_price"] == null)
                {
                    response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                    response.Message = "Giá mua không được để trống";
                    response.Data = null;
                    return Ok(response);
                }
                if (streamProvider.FormData["pu_tax"] == null)
                {
                    response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                    response.Message = "Thuế không được để trống";
                    response.Data = null;
                    return Ok(response);
                }

                if (streamProvider.FormData["pu_unit"] == null)
                {
                    response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                    response.Message = "Đơn vị không được để trống";
                    response.Data = null;
                    return Ok(response);
                }
                if (streamProvider.FormData["product_category_id"] == null)
                {
                    response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                    response.Message = "Loại sản phẩm không được để trống";
                    response.Data = null;
                    return Ok(response);
                }
                if (streamProvider.FormData["provider_id"] == null)
                {
                    response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                    response.Message = "Nhà cung cấp không được để trống";
                    response.Data = null;
                    return Ok(response);
                }

                
                // get data from formdata
                ProductUpdateViewModel productUpdateViewModel = new ProductUpdateViewModel
                {
                    pu_id = Convert.ToInt32(streamProvider.FormData["pu_id"]),
                    pu_name = Convert.ToString(streamProvider.FormData["pu_name"]),

                    pu_quantity = Convert.ToInt32(streamProvider.FormData["pu_quantity"]),
                    pu_buy_price = Convert.ToInt32(streamProvider.FormData["pu_buy_price"]),
                    pu_sale_price = Convert.ToInt32(streamProvider.FormData["pu_sale_price"]),

                    product_category_id = Convert.ToInt32(streamProvider.FormData["product_category_id"]),
                    provider_id = Convert.ToInt32(streamProvider.FormData["provider_id"]),
                    pu_tax = Convert.ToInt32(streamProvider.FormData["pu_tax"]),
                    pu_unit = Convert.ToByte(streamProvider.FormData["pu_unit"]),

                };
                //Lấy ra dữ liệu cũ 
                var existproduct = _productservice.Find(productUpdateViewModel.pu_id);
                //Kiem tra cac truong con lại 
                //Nếu là datetime, option  mà null thì cập nhập lại cái cũ 
                //Các trường khác thì trả về null 
                if (streamProvider.FormData["pu_update_date"] == null)
                {
                    if (existproduct.pu_update_date != null)
                    {
                        productUpdateViewModel.pu_update_date = existproduct.pu_update_date;
                    }
                    else
                    {
                        productUpdateViewModel.pu_update_date = null;
                    }
                }
                else
                {
                    productUpdateViewModel.pu_update_date = Convert.ToDateTime(streamProvider.FormData["pu_update_date"]);
                }

                if (streamProvider.FormData["pu_saleoff"] == null)
                {
                    
                    productUpdateViewModel.pu_saleoff = null;
                    
                }
                else
                {
                    productUpdateViewModel.pu_saleoff = Convert.ToInt32(streamProvider.FormData["pu_saleoff"]);
                }

                if (streamProvider.FormData["pu_weight"] == null)
                {
                    productUpdateViewModel.pu_weight = null;
                }
                else
                {
                    productUpdateViewModel.pu_weight = Convert.ToInt32(streamProvider.FormData["pu_weight"]);
                }

                if (streamProvider.FormData["pu_description"] == null)
                {
                    productUpdateViewModel.pu_description = null;
                }
                else
                {
                    productUpdateViewModel.pu_description = Convert.ToString(streamProvider.FormData["pu_description"]);
                }

                if (streamProvider.FormData["pu_short_description"] == null)
                {
                    productUpdateViewModel.pu_short_description = null;
                }
                else
                {
                    productUpdateViewModel.pu_short_description = Convert.ToString(streamProvider.FormData["pu_short_description"]);
                }
                //Cap nhập lại date code 
                productUpdateViewModel.pu_create_date = existproduct.pu_create_date;
                productUpdateViewModel.pu_code = existproduct.pu_code;
                // mapping view model to entity
                var updatedproduct = _mapper.Map<product>(productUpdateViewModel);



                // update product
                _productservice.Update(updatedproduct, productUpdateViewModel.pu_id);
                // return response
                response.Code = HttpCode.OK;
                response.Message = MessageResponse.SUCCESS;
                response.Data = updatedproduct;
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

        #region [Delete]
        [HttpDelete]
        [Route("api/products/delete")]
        public IHttpActionResult Deleteproduct(int productId)
        {
            ResponseDataDTO<product> response = new ResponseDataDTO<product>();
            try
            {
                var productDeleted = _productservice.Find(productId);
                if (productDeleted != null)
                {
                    _productservice.Delete(productDeleted);

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
                    response.Message = "Không thấy được sản phẩm";
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

        #region["Update Avatar"]
        [HttpPut]
        [Route("api/products/update_image")]
        public async Task<IHttpActionResult> UpdateAvatar()
        {
            ResponseDataDTO<string> response = new ResponseDataDTO<string>();
            try
            {
                var path = Path.GetTempPath();

                if (!Request.Content.IsMimeMultipartContent("form-data"))
                {
                    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.UnsupportedMediaType));
                }

                MultipartFormDataStreamProvider streamProvider = new MultipartFormDataStreamProvider(path);

                await Request.Content.ReadAsMultipartAsync(streamProvider);
                // save file
                string fileName = "";
                if (streamProvider.FormData["pu_id"] == null)
                {
                    response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                    response.Message = "Ma id không được để trống";
                    response.Data = null;
                    return Ok(response);
                }
                var pu_id = int.Parse(streamProvider.FormData["pu_id"]);
                var product = _productservice.Find(pu_id);
                if (product == null)
                {
                    response.Code = HttpCode.NOT_FOUND;
                    response.Message = MessageResponse.FAIL;
                    response.Data = null;
                    return Ok(response);
                }
                foreach (MultipartFileData fileData in streamProvider.FileData)
                {
                    fileName = FileExtension.SaveFileProductOnDisk(fileData, product.pu_code);
                }
                product.pu_thumbnail = fileName;
                _productservice.Update(product, pu_id);
                response.Code = HttpCode.OK;
                response.Message = MessageResponse.SUCCESS;
                response.Data = fileName;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Code = HttpCode.INTERNAL_SERVER_ERROR;
                response.Message = ex.Message;
                response.Data = null;
                return Ok(response);
            }
        }
        #endregion

        #region["Export Excel"]
        [HttpGet]
        [Route("api/product/export")]
        public async Task<IHttpActionResult> ExportProduct(int pageNumber, int pageSize, string search_name, int? category_id)
        {
            ResponseDataDTO<string> response = new ResponseDataDTO<string>();
            try
            {
                var listProduct = new List<productview>();

                //Đưa ra danh sách staff trong trang nào đó 
                var objRT_Mst_Product = _productservice.ExportProduct(pageNumber, pageSize, search_name, category_id);
                if (objRT_Mst_Product != null)
                {
                    listProduct.AddRange(objRT_Mst_Product.Results);

                    Dictionary<string, string> dicColNames = GetImportDicColums();

                    string url = "";
                    string filePath = GenExcelExportFilePath(string.Format(typeof(product).Name), ref url);

                    ExcelExport.ExportToExcelFromList(listProduct, dicColNames, filePath, string.Format("Products"));
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
                 {"pu_code","MSP" },
                 {"pu_name","Tên sản phẩm"},
                 {"pu_quantity","Số lượng"},
                 {"pu_buy_price","Giá mua"},
                 {"pu_sale_price","Giá bán"},
                 {"product_category_name","Thể loại"},
                 {"provider_name","Nhà cung cấp"},
                 {"pu_unit_name","Đơn vị"},
                 {"pu_saleoff","Khuyễn mãi"},
                 {"pu_short_description","Mô tả ngắn"},
                 {"pu_create_date","Ngày tạo"},
                 {"pu_update_date","Ngày cập nhập"},
                 {"pu_description","Mô tả chi tiết"},
                 {"pu_tax","Thuế"},
                 {"pu_expired_date","Ngày hết hạn"},
                 {"pu_weight","Cân nặng"}

            };
        }
        #endregion
        #region dispose

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _productservice.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}