using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Maui.BestPrice.App.Models;
using Maui.BestPrice.App.Searchers.Types;

namespace Maui.BestPrice.App.Searchers;

public class DrogaRaiaSearcher : IMedicineSearcher
{
    const string URL = @"https://app-api-m2-prod.drogaraia.com.br/graphql";

    public DrogaRaiaSearcher()
    {
    }

    public async Task<IEnumerable<Medicine>> SearchAsync(string searchTerm)
    {
        var httpClient = new HttpClient();
        //httpClient.DefaultRequestHeaders.Add("sec-ch-ua-platform", "iOS");
        httpClient.DefaultRequestHeaders.Add("x-api-key", "5f308895c59fadb0b9ed43341c6eb33e41e78394d3ca970c5a285e91d25bc9cd");

        var content = new StringContent(string.Format(""""{\"operationName\":\"Search\",\"variables\":{\"searchInput\":{\"term\":\"imussuprex\",\"pagination\":{\"page\":0,\"window\":10},\"stixAcceleratorId\":null,\"enableMarketplace\":true,\"searchApiVersion\":\"LINX\",\"branchIdBranchStockId\":null}},\"query\":\"query Search($searchInput: SearchInput!, $isStixNewAccelerator: Boolean, $token: String) {  search(search: $searchInput, token: $token, isStixNewAccelerator: $isStixNewAccelerator) {    ...SearchResult    __typename  }}fragment SearchResult on SearchResult {  sorts {    ...Sort    __typename  }  filters {    ...Filter    __typename  }  pagination {    ...Pagination    __typename  }  products {    ...Product    __typename  }  ofexs {    ...Ofex    __typename  }  __typename}fragment Sort on Sort {  id  label  ascending  applied  __typename}fragment Filter on Filter {  id  label  values {    ...FilterValue    __typename  }  __typename}fragment FilterValue on FilterValue {  id  label  applied  __typename}fragment Pagination on Pagination {  page  window  total  totalPages  hasNextPage  __typename}fragment Product on Product {  sku  skuString  name  image  gallery  availability {    ...Availability    __typename  }  description  isOTC  price {    ...Price    __typename  }  hasInmetroStamp  oldPrice {    ...Price    __typename  }  savingPercentage {    ...Percentage    __typename  }  savingPrice {    ...Price    __typename  }  lmpm {    ...Lmpm    __typename  }  packageQty  brand  manufacturer  dosage  ms  leafletUrl  activeIngredient  ofex {    ...Ofex    __typename  }  nrp  pbm {    ...PbmProduct    __typename  }  trackingUrl  qty  maxQty  totalPrice {    ...Price    __typename  }  prescriptionId  isPrescription  isBlackFriday  cdSubCategoria  codTarja  seller {    ...SimpleSeller    __typename  }  subCategoria  categoria  ratings {    ...ProductRatings    __typename  }  ratingsHistogram {    ...ProductRatingsHistogram    __typename  }  bestPrice {    description    discountType    discountValue    totalPrice    discountPercent    __typename  }  NRPCouponDrogasil {    ...NRP    __typename  }  pbmExternalSignUpUrl  grupoUnivers  clickUrl  discountRulesPbm {    discountType    discountRules {      discount      quantity      valueWithDiscount      valueWithoutDiscount      totalSavingsLabel      ruleDiscountLabel      ruleDiscountDescription      __typename    }    __typename  }  isUnivers  __typename}fragment Availability on Availability {  isSellable  hasStock  isControlled  __typename}fragment Price on Price {  value  label  __typename}fragment Percentage on Percentage {  value  label  __typename}fragment Lmpm on Lmpm {  price {    ...Price    __typename  }  qty  savingPercentage {    ...Percentage    __typename  }  __typename}fragment Ofex on Ofex {  type  typeDescription  sectionId  code  cds  nr  description  savingPercentage {    ...Percentage    __typename  }  savingPriceV2 {    ...OfexSavingPrice    __typename  }  combined {    ...OfexCombined    __typename  }  lmpm {    ...Lmpm    __typename  }  product {    ...CodeName    __typename  }  images  brands  isActivated  endDate  qtyProducts  categoryName  productName  __typename}fragment OfexSavingPrice on OfexSavingPrice {  priceValue {    ...Price    __typename  }  minPrice {    ...Price    __typename  }  __typename}fragment OfexCombined on OfexCombined {  productCombinedCode  mainName  mainImage  secondaryName  secondaryImage  __typename}fragment CodeName on CodeName {  code  name  __typename}fragment PbmProduct on PbmProduct {  name  logo  minPrice {    ...Price    __typename  }  idPbm  savingPercentage {    ...Percentage    __typename  }  isApplied  pbmPoints  discountPricePbm  __typename}fragment SimpleSeller on Seller {  id  name  __typename}fragment ProductRatings on ProductRatings {  count  average  __typename}fragment ProductRatingsHistogram on ProductRatingsHistogram {  histogram {    one    two    three    four    five    __typename  }  average  best  count  __typename}fragment NRP on NRP {  cds  savingPercentage {    ...Percentage    __typename  }  savingPrice {    ...Price    __typename  }  __typename}\"}"""", Encoding.UTF8, "application/json", searchTerm));

        var response = await httpClient.PostAsync(string.Format(URL, searchTerm), content);

        if (response.IsSuccessStatusCode)
        {
            //var a = await response.Content.ReadAsStringAsync();
            var medicinesRaw = (JsonElement)JsonSerializer.Deserialize<dynamic>(await response.Content.ReadAsStringAsync());
            var medicines = JsonSerializer.Deserialize<IList<Medicine>>(medicinesRaw.GetProperty("data").GetProperty("search").GetProperty("products"));

            foreach (var medicine in medicines)
            {
                medicine.DrugStore = "Droga Raia";
            }

            return medicines;

        }
        else
        {
            return default(IEnumerable<Medicine>);
        }
    }
}

