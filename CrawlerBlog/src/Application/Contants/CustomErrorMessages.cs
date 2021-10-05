using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contants
{
    public static class CustomErrorMessages
    {
        public const string NoPostToUpdate = "No posts are update";
        public const string InvaildPageInput = "pageIndex and pageSize must be positive value";
        public const string NodeReturnNull = "Classes changed or can not get html";
        public const string CmtAPIChanged = "API changed or wrong cmtId";
    }
}
