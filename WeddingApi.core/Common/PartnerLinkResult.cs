namespace WeddingApi.core.Common
{
    public enum PartnerLinkResult
    {
        Success,
        InvalidCode,      // الكود غلط أو منتهي أو اتستخدم
        SelfInvite,       // العميل حاول يستخدم كوده هو
        ClientNotFound,
        AlreadyLinked,    // أحد الطرفين مربوط بالفعل
        SameGender        // لازم عريس وعروسة
    }
}
