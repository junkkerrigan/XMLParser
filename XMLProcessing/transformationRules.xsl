<s:stylesheet
    xmlns:s="http://www.w3.org/1999/XSL/Transform"
    version="1.0"
>
    <s:template match="*">
        <s:copy>
            <s:copy-of select="@*"/>
            <s:apply-templates/>
        </s:copy>
    </s:template>
</s:stylesheet>