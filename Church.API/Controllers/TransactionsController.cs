using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Church.API.Data.DBContext;
using Church.API.Models;
using Newtonsoft.Json;
using Church.API.Models.AppModel.Request.Transactions;
using MySql.Data.MySqlClient;
using System.Data;
using Church.API.Models.AppModel.Response.Transactions;
using Church.API.Models.AppModel.Response;

namespace Church.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly IronChurchContext _context;

        public TransactionsController(IronChurchContext context)
        {
            _context = context;
        }

        // GET: api/Transactions
        [HttpGet]
        [Route("SearchTransactions/{page}/{limit}")]
        public List<Contribution> SearchTransactions(int page, int limit)
        {
            if (page == 0)
                page = 1;

            if (limit == 0)
                limit = int.MaxValue;

            var skip = (page - 1) * limit;

            return _context.Contribution
                .Include(mem => mem.Contributor)
                .OrderByDescending(x => x.TransactionDate)
                .Skip(skip)
                .Take(limit)
                .ToList();
        }

        // GET: api/Transactions
        [HttpPost]
        [Route("SearchTransactions")]
        public GenericSearchResponse<List<SearchTransactionsResponse>> SearchTransactions([FromBody] SearchTransactionsRequest searchTransactionsRequest)
        {
            string orderByClause = string.Empty;
            string limitClause = string.Empty; 
            string sWhere = "cn.status = 1";

            if (!string.IsNullOrWhiteSpace(searchTransactionsRequest.OrganizationId?.ToString()))
            {
                if (!string.IsNullOrWhiteSpace(sWhere))
                    sWhere += " AND ";
                sWhere += $"acc.organization_id = {searchTransactionsRequest.OrganizationId}";
            }

            if (!string.IsNullOrWhiteSpace(searchTransactionsRequest.MemberPayeeId?.ToString()))
            {
                if (!string.IsNullOrWhiteSpace(sWhere))
                    sWhere += " AND ";
                sWhere += $"cn.contributor_id = {searchTransactionsRequest.MemberPayeeId}";
            }

            if (!string.IsNullOrWhiteSpace(searchTransactionsRequest.TransactionName))
            {
                if (!string.IsNullOrWhiteSpace(sWhere))
                    sWhere += " AND ";
                sWhere += $"cn.contribution_name = '{searchTransactionsRequest.TransactionName}'";
            }

            if (!string.IsNullOrWhiteSpace(searchTransactionsRequest.AccountId?.ToString()))
            {
                if (!string.IsNullOrWhiteSpace(sWhere))
                    sWhere += " AND ";
                sWhere += $"acc.account_id = {searchTransactionsRequest.AccountId}";
            }

            if (!string.IsNullOrWhiteSpace(searchTransactionsRequest.TransactionType?.ToString()))
            {
                if (!string.IsNullOrWhiteSpace(sWhere))
                    sWhere += " AND ";
                sWhere += $"cn.transaction_type = {searchTransactionsRequest.TransactionType}";
            }

            if (!string.IsNullOrWhiteSpace(searchTransactionsRequest.TransactionMode?.ToString()))
            {
                if (!string.IsNullOrWhiteSpace(sWhere))
                    sWhere += " AND ";
                sWhere += $"cn.transaction_mode = {searchTransactionsRequest.TransactionMode}";
            }

            if (!string.IsNullOrWhiteSpace(searchTransactionsRequest.Category?.ToString()))
            {
                if (!string.IsNullOrWhiteSpace(sWhere))
                    sWhere += " AND ";
                sWhere += $"cn.category = {searchTransactionsRequest.Category}";
            }

            if (!string.IsNullOrWhiteSpace(searchTransactionsRequest.CheckNumber))
            {
                if (!string.IsNullOrWhiteSpace(sWhere))
                    sWhere += " AND ";
                sWhere += $"cn.check_number = '{searchTransactionsRequest.CheckNumber}'";
            }

            if (!string.IsNullOrWhiteSpace(searchTransactionsRequest.FromDate.ToString()))
            {
                if (!string.IsNullOrWhiteSpace(sWhere))
                    sWhere += " AND ";

                sWhere += $"date_format(cn.transaction_date, '%Y-%m-%d') >= '{searchTransactionsRequest.FromDate.Value.ToString("yyyy-MM-dd")}'";
            }

            if (!string.IsNullOrWhiteSpace(searchTransactionsRequest.ToDate.ToString()))
            {
                if (!string.IsNullOrWhiteSpace(sWhere))
                    sWhere += " AND ";

                sWhere += $"date_format(cn.transaction_date, '%Y-%m-%d') <= '{searchTransactionsRequest.ToDate.Value.ToString("yyyy-MM-dd")}'";
            }

            sWhere = $" WHERE {sWhere} ";
            
            if (!string.IsNullOrWhiteSpace(searchTransactionsRequest.SearchParameters?.OrderBy))
                orderByClause = $"ORDER BY {searchTransactionsRequest.SearchParameters?.OrderBy} {searchTransactionsRequest.SearchParameters?.SortOrder}";

            limitClause = $"LIMIT {searchTransactionsRequest.SearchParameters?.StartAt}, {searchTransactionsRequest.SearchParameters?.MaxRecords}";

            var transactionList = new List<SearchTransactionsResponse>();
            // var ContributionList = new List<Contribution>();
            int totalRows = 0;
            
            using (MySqlConnection connection = (MySqlConnection)_context.Database.GetDbConnection())
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $@"SELECT sql_calc_found_rows cn.contribution_id AS 'Contribution Id',
       cn.contributor_id,
       acc.account_name,
       cr.first_name,
       cr.last_name,
       cr.family_name,
       cn.account_id as 'AccountId',
       CASE
          WHEN IFNULL(cn.contribution_name, '') = ''
          THEN
             CONCAT(cr.first_name, ' ', cr.last_name)
          ELSE
             cn.contribution_name
       END
          AS 'Name',
       cvd.description AS Category,
       cvd_transtype.description AS Type,
       cvd_transmode.description AS Mode,
       cn.amount AS Amount,
       cn.check_number AS 'Check #',
       cn.transaction_date AS 'Trans DT',
       cn.note AS 'Note',
       cn.date_added AS 'Date Added'
  FROM contribution cn
       LEFT JOIN account acc
          ON acc.account_id = cn.account_id AND acc.status = 1
       LEFT JOIN organization org
          ON org.organization_id = acc.organization_id
       LEFT JOIN contributor cr ON cr.contributor_id = cn.contributor_id
       LEFT JOIN table_column tc
          ON     tc.table_name = 'contribution'
             AND tc.column_name = 'category'
             AND tc.status = 1
       LEFT JOIN column_value_desc cvd
          ON     cvd.table_column_id = tc.table_column_id
             AND cvd.value = cn.category
             AND cvd.status = 1
       LEFT JOIN table_column tc_transtype
          ON     tc_transtype.table_name = 'contribution'
             AND tc_transtype.column_name = 'transaction_type'
             AND tc_transtype.status = 1
       LEFT JOIN column_value_desc cvd_transtype
          ON     cvd_transtype.table_column_id = tc_transtype.table_column_id
             AND cvd_transtype.value = cn.transaction_type
             AND cvd_transtype.status = 1
       LEFT JOIN table_column tc_transmode
          ON     tc_transmode.table_name = 'contribution'
             AND tc_transmode.column_name = 'transaction_mode'
             AND tc_transmode.status = 1
       LEFT JOIN column_value_desc cvd_transmode
          ON     cvd_transmode.table_column_id = tc_transmode.table_column_id
             AND cvd_transmode.value = cn.transaction_mode
             AND cvd_transmode.status = 1
 -- ORDER BY cn.date_added DESC; 
{sWhere} {orderByClause} {limitClause};
select found_rows() as total_records";

                    try
                    {
                        MySqlDataAdapter sda = new MySqlDataAdapter();
                        sda.SelectCommand = command;
                        
                        DataSet ds = new DataSet();
                        sda.Fill(ds);

                        System.Data.DataTable dt = ds.Tables[0];
                        totalRows = int.Parse(ds.Tables[1].Rows[0]["total_records"].ToString());

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            foreach (DataRow dRow in dt.Rows)
                            {
                                transactionList.Add(new SearchTransactionsResponse(dRow));

                                /*ContributionList.Add(new Contribution()
                                {
                                    ContributionName = dRow["Name"].ToString(),
                                    ContributorId = int.TryParse(dRow["contributor_id"].ToString(), out int conId) ? int.Parse(dRow["contributor_id"].ToString()) : (int?)null,
                                    Contributor = int.TryParse(dRow["contributor_id"].ToString(), out int contId) ? new Contributor()
                                    {
                                        ContributorId = int.Parse(dRow["contributor_id"].ToString()),
                                        FirstName = dRow["first_name"].ToString(),
                                        LastName = dRow["last_name"].ToString(),
                                        FamilyName = dRow["family_name"].ToString()
                                    } : null,
                                    Amount = Convert.ToDecimal(dRow["Amount"].ToString()),
                                    Category = dRow["family_name"].ToString()
                                });*/

                                /*this.ContributionId = dRow["Contribution Id"].ToString();
                                this.AccountId = dRow["AccountId"].ToString();
                                this.AccountName = dRow["account_name"].ToString();
                                this.ContributorName = dRow["Name"].ToString();
                                this.Category = dRow["Category"].ToString();
                                this.TransactionType = dRow["Type"].ToString();
                                this.TransactionMode = dRow["Mode"].ToString();
                                this.Amount = dRow["Amount"].ToString();
                                this.CheckNumber = dRow["Check #"].ToString();
                                this.TransactionDate = Convert.ToDateTime(dRow["Trans DT"].ToString());
                                this.Note = dRow["Note"].ToString();
                                this.DateAdded = Convert.ToDateTime(dRow["Date Added"].ToString());*/
                            }
                        }
                    }
                    finally
                    {
                    }
                }
            }

            var SearchTransactionResponseList = new GenericSearchResponse<List<SearchTransactionsResponse>>()
            {
                Response = transactionList,
                TotalRecordCount = totalRows
            };

            return SearchTransactionResponseList;
        }

        [HttpGet]
        public async Task<List<Contribution>> Get()
        {
            return await _context.Contribution
                .Include(trans => trans.Contributor)
                .Include(acc => acc.Account)
                .Where(x => x.Status == 1)
                .ToListAsync();
        }

        // GET: api/Transactions/5
        [HttpGet("{id}")]
        public Contribution GetContribution(int id)
        {
            var contribution = _context.Contribution
                .Where(x => x.ContributionId == id)
                .Include(mem => mem.Contributor)
                .Include(acc => acc.Account)
                .FirstOrDefault();

            if (contribution == null)
            { }

            return contribution;
        }

        // GET: api/Transactions/5
        [HttpGet]
        [Route("GetAccountBalance")]
        [Produces(typeof(Dictionary<string, decimal>))]
        public Dictionary<string, decimal> GetAccountBalance()
        {
            var accountBalanceList = new Dictionary<string, decimal>();

            var accountList = _context.Account.Where(x => x.Status == 1)
                .ToList();

            foreach (var account in accountList)
            {
                var creditAmount = _context.Contribution
                    .Where(c => c.AccountId == account.AccountId && c.Status == 1 && c.TransactionType == 1)
                    .Sum(x => x.Amount);

                var debitAmount = _context.Contribution
                    .Where(c => c.AccountId == account.AccountId && c.Status == 1 && c.TransactionType == 2)
                    .Sum(x => x.Amount);

                var initialBalanceOnAccount = _context.Account
                    .Where(x => x.AccountId == account.AccountId && x.Status == 1)
                    .Select(x => x.InitialBalance)
                    .FirstOrDefault();

                var totalBalance = (creditAmount + initialBalanceOnAccount) - debitAmount;

                accountBalanceList.Add(account.AccountName, totalBalance);
            }

            return accountBalanceList;
        }

        // PUT: api/Transactions/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutContribution(int id, Contribution contribution)
        //{
        //    if (id != contribution.ContributionId)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(contribution).Property(x => x.ContributionName).IsModified = true;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!ContributionExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        // POST: api/Transactions
        //[HttpPost]
        //public async Task<ActionResult<Contribution>> PostContribution(Contribution contribution)
        //{
        //    _context.Contribution.Add(contribution);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetContribution", new { id = contribution.ContributionId }, contribution);
        //}

        // DELETE: api/Transactions/5
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<Contribution>> DeleteContribution(int id)
        //{
        //    var contribution = await _context.Contribution.FindAsync(id);
        //    if (contribution == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Contribution.Remove(contribution);
        //    await _context.SaveChangesAsync();

        //    return contribution;
        //}

        private bool ContributionExists(int id)
        {
            return _context.Contribution.Any(e => e.ContributionId == id);
        }
    }
}
