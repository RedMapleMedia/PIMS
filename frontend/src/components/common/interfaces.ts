export interface BasePropertyFilter {
  searchBy: string;
  pid: string;
  address: string;
  administrativeArea: string;
  projectNumber: string;
  /** comma-separated list of agencies to filter by */
  agencies: string;
  classificationId: string;
  minLotSize: string;
  maxLotSize: string;
}
