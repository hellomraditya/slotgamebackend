﻿using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using SlotGameBackend.Models;
using System.Transactions;

namespace SlotGameBackend.Services
{
    public interface IWalletService
    {
        void CreateTransaction(int amount, TransactionType type);

        IEnumerable<TransactionResponse> GetPendingTransactions(int pageNumber,int pageSize);

        Task<bool> ApproveTransaction(Guid transactionId);

        Task<bool> RejectTransaction(Guid transactionId);

        IEnumerable<TransactionResponse> GetTransactionHistory(Guid? userId,int pageNumber,int pageSize);

        IEnumerable<TransactionResponse> SearchTransaction(QueryType type, int pageNumber, int pageSize);

        Task<int> checkBalance();

    }
}
