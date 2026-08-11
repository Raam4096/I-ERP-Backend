namespace iERP.SharedKernel.Exceptions;

public static class ErrorCodes
{
    public const string ValidationError = "VALIDATION_ERROR";
    public const string DuplicateRecord = "DUPLICATE_RECORD";
    public const string BusinessRuleViolation = "BUSINESS_RULE_VIOLATION";
    public const string InvalidStatusTransition = "INVALID_STATUS_TRANSITION";
    public const string DocumentAlreadyPosted = "DOCUMENT_ALREADY_POSTED";
    public const string CreditLimitExceeded = "CREDIT_LIMIT_EXCEEDED";
    public const string InsufficientStock = "INSUFFICIENT_STOCK";

    public const string Unauthorized = "UNAUTHORIZED";
    public const string TokenExpired = "TOKEN_EXPIRED";
    public const string Forbidden = "FORBIDDEN";
    public const string TenantNotFound = "TENANT_NOT_FOUND";
    public const string TenantSuspended = "TENANT_SUSPENDED";
    public const string FieldPermissionDenied = "FIELD_PERMISSION_DENIED";

    public const string AiPermissionDenied = "AI_PERMISSION_DENIED";
    public const string AiApprovalRequired = "AI_APPROVAL_REQUIRED";
    public const string AiRollbackFailed = "AI_ROLLBACK_FAILED";

    public const string WorkflowError = "WORKFLOW_ERROR";
    public const string BridgeConditionNotMet = "BRIDGE_CONDITION_NOT_MET";
    public const string NotFound = "NOT_FOUND";
    public const string InternalError = "INTERNAL_ERROR";
}
