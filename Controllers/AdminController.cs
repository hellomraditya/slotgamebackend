﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SlotGameBackend.Models;
using SlotGameBackend.Requests;
using SlotGameBackend.Services;
using System.Collections.Generic;

namespace SlotGameBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="admin")]
    public class AdminController : ControllerBase
    {

        private readonly IWalletService _walletService;
        private readonly IAdminServices _adminServices;
        private readonly IGameService _gameService;


        public AdminController(IWalletService walletService,IAdminServices adminServices,IGameService gameService)
        {
            _walletService = walletService;
            _adminServices = adminServices;
            _gameService=gameService;
        }

        [HttpGet("pendingtransaction")]
        public ActionResult<List<TransactionResponse>> GetPendingTransactions([FromQuery]PendingRequest request)
        {
            try
            {
                var list = _walletService.GetPendingTransactions(request.pageNumber,request.pageSize);
                if (list == null)
                {
                   return NotFound( new ResponseModel
                    {
                        statusCode = 404,
                        message = "transaction not found",
                        data = "no data",
                        isSuccess = false
                    });
                }
                    return Ok(new ResponseModel
                    {
                    statusCode = 200,
                    message = "get your transactions here",
                    data = list,
                    isSuccess = true
                     });
            }catch(Exception ex)
            {
                return StatusCode(500, new ResponseModel
                {
                    statusCode = 500,
                    message = "Internal server error",
                    data = ex.InnerException?.Message ?? ex.Message,
                    isSuccess = false
                });
            }
        }
       
        [HttpPost("Approve")]
        public IActionResult Approve(ApproveRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseModel
                {
                    statusCode = 400,
                    message = "Invalid amount",
                    data = "No data",
                    isSuccess = false
                });
            }
            try
            {
                _walletService.ApproveTransaction(request.transactionId);
                return Ok(new ResponseModel
                {
                    statusCode=200,
                    message="your request has been accepted",
                    data="no data",
                    isSuccess = true
                });
            }catch(Exception ex)
            {
                return StatusCode(500, new ResponseModel
                {
                    statusCode = 500,
                    message = "Internal server error",
                    data = ex.InnerException?.Message ?? ex.Message,
                    isSuccess = false
                });
            }
        }

        [HttpPost("reject")]
        public IActionResult Reject(RejectRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseModel
                {
                    statusCode = 400,
                    message = "Invalid amount",
                    data = "No data",
                    isSuccess = false
                });
            }
            try
            {
                _walletService.RejectTransaction(request.transactionId);
                return Ok(new ResponseModel
                {
                    statusCode = 200,
                    message = " request has been rejected",
                    data = "no data",
                    isSuccess = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel
                {
                    statusCode = 500,
                    message = "Internal server error",
                    data = ex.InnerException?.Message ?? ex.Message,
                    isSuccess = false
                });
            }
        }
        [HttpGet("transactionHistory")]
        public IActionResult GetTransactionHistory([FromQuery] transHistoryRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseModel
                {
                    statusCode = 400,
                    message = "Invalid limit",
                    data = "No data",
                    isSuccess = false
                });
            }
            try
            {
                var list=_walletService.GetTransactionHistory(request.userId,request.pageNumber,request.pageSize);
                return Ok(new ResponseModel
                {
                    statusCode = 200,
                    message = "user's transaction history",
                    data = list,
                    isSuccess = true
                });
            }catch(Exception ex)
            {
                return StatusCode(500, new ResponseModel
                {
                    statusCode = 500,
                    message = "Internal server error",
                    data = ex.InnerException?.Message ?? ex.Message,
                    isSuccess = false
                });
            }
        }
        [HttpPost("setBetLimit")]
        public async Task<IActionResult> SetMinBetLimit(LimitRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseModel
                {
                    statusCode = 400,
                    message = "Invalid limit",
                    data = "No data",
                    isSuccess = false
                });
            }
            try
            {
                await _adminServices.SetMinBetLimit(request.minimumBetLimit);
                return Ok(new ResponseModel
                {
                    statusCode = 200,
                    message = "limit has been set",
                    data = "no data",
                    isSuccess = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel
                {
                    statusCode = 500,
                    message = "Internal server error",
                    data = ex.InnerException?.Message ?? ex.Message,
                    isSuccess = false
                });
            }
            
        }
        [HttpPost("AddPayline")]
        public async Task<IActionResult> AddPayLine(AddPayLineRequest request)
        {
            if (request == null || request.positions == null || request.positions.Count == 0)
            {
                return BadRequest(new ResponseModel
                {
                    statusCode = 400,
                    message = "Positions are required",
                    data = "No data",
                    isSuccess = false
                });
            }
            try
            {
                var positions = new List<Tuple<int, int>>();
                foreach (var pos in request.positions)
                {
                    positions.Add(new Tuple<int,int>(pos.X, pos.Y));
                }
                await  _adminServices.AddPayLine(positions,request.multiplier);
                return Ok(new ResponseModel
                {
                    statusCode = 200,
                    message = "payline has been added",
                    data = "no data",
                    isSuccess = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel
                {
                    statusCode = 500,
                    message = "Internal server error",
                    data = ex.InnerException?.Message ?? ex.Message,
                    isSuccess = false
                });

            }

        }
        [HttpPost("RemovePayLine")]
        public async Task<IActionResult> RemovePayline(RemovePayLineRequest request)
        {

            try
            {
                 await _adminServices.RemovePaylineAsync(request.paylineId);
                
                    return Ok(new ResponseModel
                    {
                        statusCode = 200,
                        message = "payline has been removed",
                        data = "no data",
                        isSuccess = true
                    });
            }
             catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel
                {
                    statusCode = 500,
                    message = "Internal server error",
                    data = ex.InnerException?.Message ?? ex.Message,
                    isSuccess = false
                });

            }
        }


        [HttpPut("updatemultiplier")]
        public IActionResult Setmultiplier(SetMultiplierRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseModel
                {
                    statusCode = 400,
                    message = "Invalid limit",
                    data = "No data",
                    isSuccess = false
                });
            }
            try
            {
                _adminServices.Setmultiplier(request.value, request.paylineId);
                return Ok(new ResponseModel
                {
                    statusCode = 200,
                    message = "multiplier has been updated",
                    data = "no data",
                    isSuccess = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel
                {
                    statusCode = 500,
                    message = "Internal server error",
                    data = ex.InnerException?.Message ?? ex.Message,
                    isSuccess = false
                });

            }
        }
        [HttpPost("AddSymbols")]
        public async Task<IActionResult> AddSymbolAsync(SymbolRequest request)
        {
            if (request.Image == null || request.Image.Length == 0)
            {
                return BadRequest(new ResponseModel
                {
                    statusCode = 400,
                    message = "Image file is required",
                    data = "No data",
                    isSuccess = false
                });
            }
            try
            {
                
               await _adminServices.AddSymbolAsync(request.symbolname,request.Image);
                
                return Ok(new ResponseModel
                {
                    statusCode = 200,
                    message = "symbol has been added successfully",
                    data = "no data",
                    isSuccess = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel
                {
                    statusCode = 500,
                    message = "Internal server error",
                    data = ex.InnerException?.Message ?? ex.Message,
                    isSuccess = false
                });

            }
        }
        [HttpGet("gamehistory")]
        public IActionResult gameHistory([FromQuery]gameHistoryRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseModel
                {
                    statusCode = 400,
                    message = "Invalid limit",
                    data = "No data",
                    isSuccess = false
                });
            }
            try
            {
                var list=_gameService.gamehistory(request.userId,request.pageNumber,request.pageSize);
                return Ok(new ResponseModel
                {
                    statusCode = 200,
                    message = "users game history",
                    data =list,
                    isSuccess = true
                });
            }catch(Exception ex)
            {
                return StatusCode(500, new ResponseModel
                {
                    statusCode = 500,
                    message = "Internal server error",
                    data = ex.InnerException?.Message ?? ex.Message,
                    isSuccess = false
                });
            }

        }

        [HttpGet("searchTransaction")]

        public IActionResult searchTransaction([FromQuery]SearchRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseModel
                {
                    statusCode = 400,
                    message = "Invalid limit",
                    data = "No data",
                    isSuccess = false
                });
            }

            try
            { 
               var  list = _walletService.SearchTransaction(request.type, request.pageNumber, request.pageSize);
                return Ok(new ResponseModel
                {
                    statusCode = 200,
                    message = "transaction during requested period",
                    data = list,
                    isSuccess = true
                });


            }catch(Exception ex)
            {
                return StatusCode(500, new ResponseModel
                {
                    statusCode = 500,
                    message = "Internal server error",
                    data = ex.InnerException?.Message ?? ex.Message,
                    isSuccess = false
                });
            }

        }
        [HttpGet("getsymbols")]
        public async Task<IActionResult> getSymbols()
        {
            try
            {
                var list = await _adminServices.getSymbol();
                return Ok(new ResponseModel
                {
                    statusCode = 200,
                    message = "your symbols available",
                    data = list,
                    isSuccess = true
                });

            }
            catch(Exception ex)
            {
                return StatusCode(500, new ResponseModel
                {
                    statusCode = 500,
                    message = "Internal server error",
                    data = ex.InnerException?.Message ?? ex.Message,
                    isSuccess = false
                });
            }
        }
        [HttpGet("getPaylines")]
        public async Task<IActionResult> getPayline()
        {
            try
            {
                var list = await _adminServices.getPayline();
                return Ok(new ResponseModel
                {
                    statusCode = 200,
                    message = "your paylines available",
                    data = list,
                    isSuccess = true
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel
                {
                    statusCode = 500,
                    message = "Internal server error",
                    data = ex.InnerException?.Message ?? ex.Message,
                    isSuccess = false
                });
            }
        }
        [HttpGet("getUsers")]
        public async Task<IActionResult> getUsers()
        {
            try
            {
                var list =await  _adminServices.getUsers();
                return Ok(new ResponseModel
                {
                    statusCode = 200,
                    message = "your user list is here",
                    data = list,
                    isSuccess = true
                });
            }catch(Exception ex)
            {
                return StatusCode(500, new ResponseModel
                {
                    statusCode = 500,
                    message = "Internal server error",
                    data = ex.InnerException?.Message ?? ex.Message,
                    isSuccess = false
                });
            }
        }
        [HttpPost("blockUser")]

        public async Task<IActionResult> blockUser(BlockRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseModel
                {
                    statusCode = 400,
                    message = "Invalid limit",
                    data = "No data",
                    isSuccess = false
                });
            }
            try
            {
                var isblock = await _adminServices.blockUser(request.userId);
                if (!isblock)
                {
                    return Conflict(new ResponseModel
                    {
                        statusCode = 409,
                        message = "already blocked",
                        data = "No data",
                        isSuccess = false
                    });
                }
                return Ok(new ResponseModel
                {
                    statusCode = 200,
                    message = "user is blocked",
                    data = "No data",
                    isSuccess = true
                });
            }catch(Exception ex)
            {
                return StatusCode(500, new ResponseModel
                {
                    statusCode = 500,
                    message = "Internal server error",
                    data = ex.InnerException?.Message ?? ex.Message,
                    isSuccess = false
                });
            }

        }
        [HttpGet("Download-gameHistory")]
        public async Task<IActionResult> DownloadGameHistory([FromQuery]downloadRequest request)
        {
            try
            {
                var excelReport = await _adminServices.GenerateGameHistoryExcelReport(request.userId,request.startDate,request.endDate);

                return File(excelReport, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "GameHistoryReport.xlsx");
               
            }catch(Exception ex)
            {
                return StatusCode(500, new ResponseModel
                {
                    statusCode = 500,
                    message = "Internal server error",
                    data = ex.InnerException?.Message ?? ex.Message,
                    isSuccess = false
                });
            }
            
        }



    }

}
