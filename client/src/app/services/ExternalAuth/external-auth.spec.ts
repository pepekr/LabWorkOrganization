import { TestBed } from '@angular/core/testing';

import { ExternalAuth } from './external-auth';

describe('ExternalAuth', () => {
  let service: ExternalAuth;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ExternalAuth);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
